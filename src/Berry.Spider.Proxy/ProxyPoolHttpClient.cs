using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace Berry.Spider.Proxy;

public class ProxyPoolHttpClient
{
    private IOptionsSnapshot<HttpProxyOptions> Options { get; }
    public HttpClient Client { get; private set; }

    public ProxyPoolHttpClient(IOptionsSnapshot<HttpProxyOptions> options, HttpClient httpClient)
    {
        this.Options = options;

        httpClient.BaseAddress = new Uri(this.Options.Value.ProxyPoolApiHost);
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