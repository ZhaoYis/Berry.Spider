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
        ChromeOptions options = new ChromeOptions();

        options.AddArgument("--no-sandbox"); //Chrome在root权限下跑
        options.AddArgument("--user-data-dir=~/usr/local/software/chrome/user_data");
        options.AddArgument("--dns-prefetch-disable");
        options.AddArgument("--lang=en-US");
        options.AddArgument("--disable-setuid-sandbox");
        options.AddArgument("--disable-gpu");
        options.AddArgument("--disable-dev-shm-usage");
        options.AddArgument("--headless"); //不用打开图形界面
        options.AddArgument("--disable-gpu");
        options.AddArgument("ignore-certificate-errors");
        options.AddArgument("blink-settings=imagesEnabled=false");

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