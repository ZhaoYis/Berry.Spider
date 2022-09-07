using Microsoft.Extensions.DependencyInjection;
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

    public ChromeOptionsProvider(ISeleniumProxyProvider proxyProvider)
    {
        this.SeleniumProxyProvider = proxyProvider;
    }

    public async Task<ChromeOptions> BuildAsync(bool isUsedProxy = true)
    {
        //https://www.cnblogs.com/gurenyumao/p/14721035.html
        ChromeOptions options = new ChromeOptions();

        //Chrome在root权限下跑
        options.AddArgument("--no-sandbox");
        // options.AddArgument("--user-data-dir=~/usr/local/software/chrome/user_data");
        options.AddArgument("--dns-prefetch-disable");
        options.AddArgument("--lang=en-US");
        options.AddArgument("--disable-setuid-sandbox");
        options.AddArgument("--disable-gpu");
        options.AddArgument("--disable-dev-shm-usage");
        //不要拦截弹出框
        // options.AddArgument("--disable-popup-blocking");
        //无界面运行(无窗口)，也叫无头浏览器，通常用于远程运行，在本地运行也可以通过该参数提升运行效率
        options.AddArgument("--headless");
        //设置浏览器以隐身模式(无痕模式运行)
        options.AddArgument("--incognito");
        options.AddArgument("ignore-certificate-errors");
        options.AddArgument("blink-settings=imagesEnabled=false");
        //擦除指纹
        options.AddArgument("disable-blink-features=AutomationControlled");
        //设置user-agent
        options.AddArgument($"user-agent={UserAgentPoolHelper.RandomGetOne()}");

        //设置代理
        if (isUsedProxy)
        {
            var proxy = await this.SeleniumProxyProvider.GetProxyAsync();
            if (proxy != null)
            {
                options.Proxy = proxy;
            }
        }

        return options;
    }
}