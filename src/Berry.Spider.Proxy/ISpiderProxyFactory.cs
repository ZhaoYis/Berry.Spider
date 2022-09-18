namespace Berry.Spider.Proxy;

public interface ISpiderProxyFactory
{
    Task<IHttpProxy> GetProxyAsync();
}