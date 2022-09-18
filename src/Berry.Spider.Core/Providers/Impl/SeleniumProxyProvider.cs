using Berry.Spider.Proxy;
using OpenQA.Selenium;

namespace Berry.Spider.Core;

/// <summary>
/// Selenium代理提供器
/// </summary>
public class SeleniumProxyProvider : ISeleniumProxyProvider
{
    private IHttpProxy HttpProxy { get; }
    private ISpiderProxyFactory _spiderProxyFactory;

    public SeleniumProxyProvider(IHttpProxy httpProxy, ISpiderProxyFactory spiderProxyFactory)
    {
        this.HttpProxy = httpProxy;
        this._spiderProxyFactory = spiderProxyFactory;
    }

    public async Task<OpenQA.Selenium.Proxy?> GetProxyAsync()
    {
        await _spiderProxyFactory.GetProxyAsync();
        
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