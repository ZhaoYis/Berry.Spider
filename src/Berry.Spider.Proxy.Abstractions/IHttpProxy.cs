namespace Berry.Spider.Proxy;

public interface IHttpProxy
{
    /// <summary>
    /// 是否有效
    /// </summary>
    public Task<bool> IsInvalid();

    /// <summary>
    /// 获取代理地址，格式：<HOST:PORT>
    /// </summary>
    /// <returns></returns>
    Task<string> GetProxyUriAsync();
}