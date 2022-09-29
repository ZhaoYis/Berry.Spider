using Microsoft.Extensions.Caching.Distributed;
using Volo.Abp.Caching;

namespace Berry.Spider.Proxy.QgNet;

public class QgNetHttpProxy : IHttpProxy
{
    private const string CacheKey = "QgNetProxyQuotaResultCacheKey";

    private QgNetProxyHttpClient QgNetProxyHttpClient { get; }
    private QgNetProxyPoolContext QgNetProxyPoolContext { get; }
    private IDistributedCache<QgNetProxyQuotaResult> Cache { get; }

    public QgNetHttpProxy(
        QgNetProxyHttpClient httpClient,
        QgNetProxyPoolContext context,
        IDistributedCache<QgNetProxyQuotaResult> cache)
    {
        this.QgNetProxyHttpClient = httpClient;
        this.QgNetProxyPoolContext = context;
        this.Cache = cache;
    }

    /// <summary>
    /// 是否有效
    /// </summary>
    public async Task<bool> IsInvalid()
    {
        QgNetProxyQuotaResult? quotaResult = await this.Cache.GetAsync(CacheKey);
        if (quotaResult != null)
        {
            return quotaResult.Available > 0;
        }
        else
        {
            quotaResult = await this.QgNetProxyHttpClient.GetQuotaResultAsync();
            if (quotaResult != null)
            {
                await this.Cache.SetAsync(CacheKey, quotaResult, new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(10)
                });

                return quotaResult.Available > 0;
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

        if (qgNetProxyResult != null) return qgNetProxyResult.Host;

        return string.Empty;
    }
}