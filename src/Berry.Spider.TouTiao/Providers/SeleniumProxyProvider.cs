using Berry.Spider.Proxy;
using OpenQA.Selenium;

namespace Berry.Spider.TouTiao;

public class SeleniumProxyProvider : ISeleniumProxyProvider
{
    private IHttpProxy HttpProxy { get; }
    
    public SeleniumProxyProvider(IHttpProxy httpProxy)
    {
        this.HttpProxy = httpProxy;
    }
    
    public async Task<OpenQA.Selenium.Proxy> GetProxyAsync()
    {
        OpenQA.Selenium.Proxy proxy = new OpenQA.Selenium.Proxy();
        
        proxy.Kind = ProxyKind.Manual;
        proxy.IsAutoDetect = false;
        proxy.HttpProxy = await this.HttpProxy.GetProxyUriAsync();

        return proxy;
    }
}