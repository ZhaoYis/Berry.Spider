namespace Berry.Spider.Proxy;

public interface IHttpProxy
{
    /// <summary>
    /// 获取代理地址，格式：<HOST:PORT>
    /// </summary>
    /// <returns></returns>
    Task<string> GetProxyUriAsync();
}