using Volo.Abp.DependencyInjection;

namespace Berry.Spider.Proxy;

public interface IHttpProxy : ITransientDependency
{
    /// <summary>
    /// 获取代理地址，格式：<HOST:PORT>
    /// </summary>
    /// <returns></returns>
    Task<string> GetProxyUriAsync();
}