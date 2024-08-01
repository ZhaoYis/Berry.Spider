using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.Caching;

namespace Berry.Spider.Proxy.QgNet;

public class QgNetHttpProxy : IHttpProxy
{
    private QgNetProxyHttpClient QgNetProxyHttpClient { get; }
    private QgNetProxyPoolContext QgNetProxyPoolContext { get; }
    private IDistributedCache<QgNetProxyQuotaResult> Cache { get; }
    private QgNetProxyOptions Options { get; }
    private ILogger<QgNetHttpProxy> Logger { get; }

    public QgNetHttpProxy(
        QgNetProxyHttpClient httpClient,
        QgNetProxyPoolContext context,
        IDistributedCache<QgNetProxyQuotaResult> cache,
        IOptionsSnapshot<QgNetProxyOptions> options,
        ILogger<QgNetHttpProxy> logger)
    {
        this.QgNetProxyHttpClient = httpClient;
        this.QgNetProxyPoolContext = context;
        this.Cache = cache;
        this.Options = options.Value;
        this.Logger = logger;
    }

    /// <summary>
    /// 是否有效
    /// </summary>
    public async Task<bool> IsInvalid()
    {
        if (this.Options.IsEnable)
        {
            QgNetProxyQuotaResult? quotaResult = await this.Cache.GetAsync(CacheKeyHelper.QgNetProxyQuotaResultCacheKey);
            if (quotaResult != null)
            {
                return quotaResult.Balance > 0;
            }
            else
            {
                quotaResult = await this.QgNetProxyHttpClient.GetQuotaResultAsync();
                if (quotaResult != null)
                {
                    await this.Cache.SetAsync(CacheKeyHelper.QgNetProxyQuotaResultCacheKey, quotaResult, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(10)
                    });

                    return quotaResult.Balance > 0;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// 获取代理地址，格式：<HOST:PORT>
    /// </summary>
    /// <returns></returns>
    public async Task<string> GetProxyUriAsync()
    {
        QgNetProxyResult? qgNetProxyResult = await this.QgNetProxyPoolContext.GetAsync();

        if (qgNetProxyResult != null)
        {
            this.Logger.LogInformation("获取到qg.net代理IP信息：{Server}", qgNetProxyResult.Server);
            return qgNetProxyResult.Server;
        }

        return string.Empty;
    }
}