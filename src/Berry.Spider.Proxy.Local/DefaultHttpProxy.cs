namespace Berry.Spider.Proxy;

public class DefaultHttpProxy : IHttpProxy
{
    private ProxyPoolHttpClient PoolHttpClient { get; }

    public DefaultHttpProxy(ProxyPoolHttpClient httpClient)
    {
        this.PoolHttpClient = httpClient;
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