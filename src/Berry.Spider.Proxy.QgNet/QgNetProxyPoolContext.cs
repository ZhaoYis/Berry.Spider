using Microsoft.Extensions.Caching.Distributed;
using Volo.Abp.Caching;

namespace Berry.Spider.Proxy.QgNet;

public class QgNetProxyPoolContext
{
    private QgNetProxyHttpClient QgNetProxyHttpClient { get; }
    private IDistributedCache<QgNetProxyResult> Cache { get; }

    public QgNetProxyPoolContext(QgNetProxyHttpClient httpClient, IDistributedCache<QgNetProxyResult> cache)
    {
        this.QgNetProxyHttpClient = httpClient;
        this.Cache = cache;
    }

    public async Task<QgNetProxyResult?> GetAsync()
    {
        QgNetProxyResult? result = await this.Cache.GetAsync(CacheKeyHelper.QgNetProxyResultCacheKey);

        //检查当前IP是否有效，无效则重新获取一个
        if (result is { IsInvalid: true })
        {
            return result;
        }
        else
        {
            result = await this.QgNetProxyHttpClient.GetOneAsync();
            await this.SetProxyResultAsync(result);

            return result;
        }
    }

    private async Task SetProxyResultAsync(QgNetProxyResult? result)
    {
        if (result == null) return;

        await this.Cache.SetAsync(CacheKeyHelper.QgNetProxyResultCacheKey, result, new DistributedCacheEntryOptions
        {
            AbsoluteExpiration = DateTime.Parse(result.Deadline)
        });
    }
}