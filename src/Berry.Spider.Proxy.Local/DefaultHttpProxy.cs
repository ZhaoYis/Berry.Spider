namespace Berry.Spider.Proxy;

public class DefaultHttpProxy : IHttpProxy
{
    private ProxyPoolHttpClient PoolHttpClient { get; }

    public DefaultHttpProxy(ProxyPoolHttpClient httpClient)
    {
        this.PoolHttpClient = httpClient;
    }

    /// <summary>
    /// 是否有效
    /// </summary>
    public Task<bool> IsInvalid()
    {
        return Task.FromResult<bool>(true);
    }

    public async Task<string> GetProxyUriAsync()
    {
        ProxyPoolResult? result = await this.PoolHttpClient.GetOneAsync();

        if (result != null)
        {
            return result.Proxy;
        }

        return string.Empty;
    }
}