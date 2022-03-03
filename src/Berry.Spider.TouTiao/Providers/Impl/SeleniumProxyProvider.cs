using Berry.Spider.Proxy;
using OpenQA.Selenium;

namespace Berry.Spider.TouTiao;

/// <summary>
/// Selenium代理提供器
/// </summary>
public class SeleniumProxyProvider : ISeleniumProxyProvider
{
    private IHttpProxy HttpProxy { get; }

    public SeleniumProxyProvider(IHttpProxy httpProxy)
    {
        this.HttpProxy = httpProxy;
    }

    public async Task<OpenQA.Selenium.Proxy?> GetProxyAsync()
    {
        OpenQA.Selenium.Proxy? proxy = new OpenQA.Selenium.Proxy();

        proxy.Kind = ProxyKind.Manual;
        proxy.IsAutoDetect = false;

        string host = await this.HttpProxy.GetProxyUriAsync();
        if (!string.IsNullOrEmpty(host))
        {
            proxy.HttpProxy = host;
            return proxy;
        }

        return default;
    }
}