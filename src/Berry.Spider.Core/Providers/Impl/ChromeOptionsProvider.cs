using Microsoft.Extensions.DependencyInjection;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.Core;

/// <summary>
/// Chrome选项提供器
/// </summary>
[Dependency(ServiceLifetime.Transient, ReplaceServices = true), ExposeServices(typeof(IDriverOptionsProvider))]
public class ChromeOptionsProvider : IDriverOptionsProvider
{
    private ISeleniumProxyProvider SeleniumProxyProvider { get; }
    private IUserAgentProvider UserAgentProvider { get; }

    public ChromeOptionsProvider(ISeleniumProxyProvider proxyProvider,
        IUserAgentProvider userAgentProvider)
    {
        this.SeleniumProxyProvider = proxyProvider;
        this.UserAgentProvider = userAgentProvider;
    }

    public async Task<ChromeOptions> BuildAsync(bool isUsedProxy = true)
    {
        //https://www.cnblogs.com/gurenyumao/p/14721035.html
        ChromeOptions options = new ChromeOptions();

        //Chrome在root权限下跑
        options.AddArgument("--no-sandbox");
        //停用DNS预读
        options.AddArgument("--dns-prefetch-disable");
        //使用指定的语言
        options.AddArgument("--lang=en-US");
        //禁用setuid沙箱（仅限Linux）
        options.AddArgument("--disable-setuid-sandbox");
        //禁用GPU硬件加速。如果软件渲染器不到位，则GPU进程将无法启动
        options.AddArgument("--disable-gpu");
        //禁用弹出拦截
        options.AddArgument("--disable-popup-blocking");
        //无界面运行(无窗口)，也叫无头浏览器，通常用于远程运行，在本地运行也可以通过该参数提升运行效率
        //在无头模式下运行，即没有UI或显示服务器依赖性。
        options.AddArgument("--headless");
        //设置浏览器以隐身模式(无痕模式运行)
        options.AddArgument("--incognito");
        //不发送 Http-Referer 头
        options.AddArgument("--no-referrers");
        //忽略与证书相关的错误
        options.AddArgument("--ignore-certificate-errors");
        //设置闪烁设置（imagesEnabled=不加载图片）
        options.AddArgument("--blink-settings=imagesEnabled=false");
        //擦除指纹
        options.AddArgument("--disable-blink-features=AutomationControlled");
        //防止信息栏出现（去掉提示：Chrome正收到自动测试软件的控制）
        options.AddArgument("--disable-infobars");
        //设置user-agent
        string userAgent = await this.UserAgentProvider.GetOnesAsync();
        options.AddArgument($"--user-agent={userAgent}");

        //TODO：自定义chrome.exe的位置？
        //https://github.com/SeleniumHQ/selenium/wiki/ChromeDriver/01fde32d0ed245141e24151f83b7c2db31d596a4#requirements
        //options.BinaryLocation = "";

        //Normal：默认值, 等待所有资源下载
        //Eager：DOM 访问已准备就绪, 但诸如图像的其他资源可能仍在加载
        //None：完全不会阻塞 WebDriver
        options.PageLoadStrategy = PageLoadStrategy.Normal;
        //检查在会话期间导航时 是否使用了过期的 (或) 无效的 TLS Certificate
        //设置为 false, 则页面浏览遇到任何域证书问题时, 将返回insecure certificate error . 如果设置为 true, 则浏览器将信任无效证书.
        options.AcceptInsecureCertificates = true;
        //设置日志级别
        options.SetLoggingPreference(LogType.Client, LogLevel.Warning);
        options.SetLoggingPreference(LogType.Driver, LogLevel.Warning);
        options.SetLoggingPreference(LogType.Browser, LogLevel.Warning);
        options.SetLoggingPreference(LogType.Server, LogLevel.Warning);
        options.SetLoggingPreference(LogType.Profiler, LogLevel.Warning);
        options.SetLoggingPreference(LogType.Performance, LogLevel.Warning);

        //设置代理
        if (isUsedProxy)
        {
            var proxy = await this.SeleniumProxyProvider.GetProxyAsync();
            if (proxy != null)
            {
                //使用指定的代理服务器，覆盖系统设置。此交换机仅影响HTTP和HTTPS请求
                //options.AddArgument($"--proxy-server={proxy.HttpProxy}");
                options.Proxy = proxy;
            }
        }

        return options;
    }
}