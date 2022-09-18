namespace Berry.Spider.Proxy;

public class SpiderProxyFactory : ISpiderProxyFactory
{
    private IEnumerable<IHttpProxy> HttpProxies { get; }

    public SpiderProxyFactory(IEnumerable<IHttpProxy> proxies)
    {
        this.HttpProxies = proxies;
    }

    public async Task<IHttpProxy> GetProxyAsync()
    {
        foreach (IHttpProxy proxy in this.HttpProxies.Reverse())
        {
            if (await proxy.IsInvalid())
            {
                return proxy;
            }
        }

        return default;
    }
}