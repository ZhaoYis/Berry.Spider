using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace Berry.Spider.Proxy.QgNet;

public class QgNetProxyHttpClient
{
    private QgNetProxyOptions Options { get; }
    private HttpClient Client { get; }
    private ILogger<QgNetProxyHttpClient> Logger { get; }

    public QgNetProxyHttpClient(IOptionsSnapshot<QgNetProxyOptions> options, HttpClient httpClient,
        ILogger<QgNetProxyHttpClient> logger)
    {
        this.Options = options.Value;
        this.Logger = logger;

        this.Client = httpClient;
        this.Client.BaseAddress = new Uri(this.Options.ProxyPoolApiHost);
    }

    /// <summary>
    /// 随机获取一个代理IP
    /// </summary>
    /// <returns></returns>
    public async Task<QgNetProxyResult?> GetOneAsync()
    {
        try
        {
            var result = await this.Client.GetFromJsonAsync<QgNetResult<List<QgNetProxyResult>>>($"/get?key={this.Options.AuthKey}&distinct=true&pool=1");
            if (result is { IsSuccess: true })
            {
                List<QgNetProxyResult> data = result.Data;
                return data.First();
            }
            else
            {
                this.Logger.LogError("代理IP获取失败：{Code}", result?.Code);
            }
        }
        catch (Exception e)
        {
            this.Logger.LogException(e);
            return default;
        }

        return default;
    }

    /// <summary>
    /// 查询可用通道数
    /// </summary>
    /// <returns></returns>
    public async Task<QgNetProxyQuotaResult?> GetQuotaResultAsync()
    {
        try
        {
            var result = await this.Client.GetFromJsonAsync<QgNetResult<QgNetProxyQuotaResult>>($"/balance?key={this.Options.AuthKey}&pool=1");
            if (result is { IsSuccess: true })
            {
                return result.Data;
            }
        }
        catch (Exception e)
        {
            return default;
        }

        return default;
    }
}