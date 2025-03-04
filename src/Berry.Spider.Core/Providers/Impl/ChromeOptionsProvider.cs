﻿using Microsoft.Extensions.DependencyInjection;
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

        //禁用沙箱模式
        options.AddArgument("--no-sandbox");
        //停用DNS预读
        options.AddArgument("--dns-prefetch-disable");
        //使用指定的语言
        options.AddArgument("--lang=en-US");
        //禁用setuid沙箱（仅限Linux）
        options.AddArgument("--disable-setuid-sandbox");
        //禁用弹出拦截
        options.AddArgument("--disable-popup-blocking");
        //禁用 Chrome 浏览器的扩展
        options.AddArgument("--disable-extensions");
        //禁用 Chrome 浏览器的通知
        options.AddArgument("--disable-notifications");
        //禁用 Chrome 浏览器的弹出窗口阻止功能
        options.AddArgument("--disable-popup-blocking");
        //禁用 Chrome 浏览器的 Web 安全功能
        options.AddArgument("--disable-web-security");
        //解决内存不足问题
        options.AddArgument("--disable-dev-shm-usage");

        //无界面运行(无窗口)，也叫无头浏览器，通常用于远程运行，在本地运行也可以通过该参数提升运行效率
        //在无头模式下运行，即没有UI或显示服务器依赖性。
        //https://developer.chrome.com/articles/new-headless/
        options.AddArgument("--headless");
        //禁用 Chrome 浏览器的 GPU 加速。解决GPU stall due to ReadPixels错误
        //https://stackoverflow.com/questions/59047415/chrome-options-in-python-selenium-disable-gpu-vs-headless
        options.AddArgument("--disable-gpu");
        //禁用3D
        options.AddArgument("--disable-3d-apis");

        //设置浏览器以隐身模式(无痕模式运行)
        options.AddArgument("--incognito");
        //不发送 Http-Referer 头
        // options.AddArgument("--no-referrers");
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
        options.PageLoadStrategy = PageLoadStrategy.Eager;
        //检查在会话期间导航时 是否使用了过期的 (或) 无效的 TLS Certificate
        //设置为 false, 则页面浏览遇到任何域证书问题时, 将返回insecure certificate error . 如果设置为 true, 则浏览器将信任无效证书.
        options.AcceptInsecureCertificates = true;

        //设置代理
        if (isUsedProxy)
        {
            var proxy = await this.SeleniumProxyProvider.GetProxyAsync();
            if (proxy != null)
            {
                //使用指定的代理服务器，覆盖系统设置。此交换机仅影响HTTP和HTTPS请求
                // options.AddArgument($"--proxy-server={proxy.HttpProxy}");
                options.Proxy = proxy;
            }
        }

        return options;
    }
}