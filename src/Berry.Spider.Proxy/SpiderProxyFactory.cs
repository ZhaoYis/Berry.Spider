namespace Berry.Spider.Proxy;

public class SpiderProxyFactory : ISpiderProxyFactory
{
    private IEnumerable<IHttpProxy> HttpProxies { get; }

    public SpiderProxyFactory(IEnumerable<IHttpProxy> proxies)
    {
        this.HttpProxies = proxies;
    }

    public Task<IHttpProxy> GetProxyAsync()
    {
        IHttpProxy httpProxy = this.HttpProxies.Reverse().First();

        return Task.FromResult<IHttpProxy>(httpProxy);
    }
}