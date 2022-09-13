using Microsoft.Extensions.Logging;

namespace Berry.Spider.Proxy;

public class DefaultHttpProxy : IHttpProxy
{
    private ILogger<DefaultHttpProxy> Logger { get; }
    private ProxyPoolHttpClient PoolHttpClient { get; }

    public DefaultHttpProxy(ILogger<DefaultHttpProxy> logger, ProxyPoolHttpClient httpClient)
    {
        this.Logger = logger;
        this.PoolHttpClient = httpClient;
    }

    public async Task<string> GetProxyUriAsync()
    {
        ProxyPoolResult? result = await this.PoolHttpClient.GetOneAsync();

        if (result != null)
        {
            this.Logger.LogInformation($"获取到代理IP信息：{result.Proxy}");
            return result.Proxy;
        }

        return string.Empty;
    }
}