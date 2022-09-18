using Berry.Spider.Proxy;
using OpenQA.Selenium;

namespace Berry.Spider.Core;

/// <summary>
/// Selenium代理提供器
/// </summary>
public class SeleniumProxyProvider : ISeleniumProxyProvider
{
    private ISpiderProxyFactory SpiderProxyFactory { get; }

    public SeleniumProxyProvider(ISpiderProxyFactory spiderProxyFactory)
    {
        this.SpiderProxyFactory = spiderProxyFactory;
    }

    public async Task<OpenQA.Selenium.Proxy?> GetProxyAsync()
    {
        IHttpProxy httpProxy = await this.SpiderProxyFactory.GetProxyAsync();
        if (httpProxy != null)
        {
            string host = await httpProxy.GetProxyUriAsync();
            if (!string.IsNullOrEmpty(host))
            {
                OpenQA.Selenium.Proxy? proxy = new OpenQA.Selenium.Proxy();
                proxy.Kind = ProxyKind.Manual;
                proxy.IsAutoDetect = false;
                proxy.HttpProxy = host;

                return proxy;
            }
        }

        return default;
    }
}