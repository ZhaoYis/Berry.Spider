using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace Berry.Spider.Proxy;

public class ProxyPoolHttpClient
{
    private HttpProxyOptions Options { get; }
    public HttpClient Client { get; private set; }

    public ProxyPoolHttpClient(IOptions<HttpProxyOptions> options, HttpClient httpClient)
    {
        this.Options = options.Value;

        httpClient.BaseAddress = new Uri(this.Options.ProxyPoolApiHost);
        this.Client = httpClient;
    }

    /// <summary>
    /// 随机获取一个代理IP
    /// </summary>
    /// <returns></returns>
    public async Task<ProxyPoolResult?> GetOneAsync()
    {
        var result = await this.Client.GetFromJsonAsync<ProxyPoolResult>("/get");

        return result;
    }
}