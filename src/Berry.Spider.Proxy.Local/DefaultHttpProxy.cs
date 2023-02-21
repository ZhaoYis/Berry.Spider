using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Berry.Spider.Proxy;

public class DefaultHttpProxy : IHttpProxy
{
    private ILogger<DefaultHttpProxy> Logger { get; }
    private ProxyPoolHttpClient PoolHttpClient { get; }
    private IOptionsSnapshot<HttpProxyOptions> Options { get; }

    public DefaultHttpProxy(ILogger<DefaultHttpProxy> logger, ProxyPoolHttpClient httpClient,
        IOptionsSnapshot<HttpProxyOptions> options)
    {
        this.Logger = logger;
        this.PoolHttpClient = httpClient;
        this.Options = options;
    }

    /// <summary>
    /// 是否有效
    /// </summary>
    public Task<bool> IsInvalid()
    {
        if (this.Options.Value.IsEnable)
        {
            return Task.FromResult<bool>(true);
        }

        return Task.FromResult<bool>(false);
    }

    public async Task<string> GetProxyUriAsync()
    {
        ProxyPoolResult? result = await this.PoolHttpClient.GetOneAsync();

        if (result != null)
        {
            this.Logger.LogInformation($"获取到私有部署代理IP信息：{result.Proxy}");
            return result.Proxy;
        }

        return string.Empty;
    }
}