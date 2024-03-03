using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.Core;

/// <summary>
/// https://www.useragents.me/api
/// </summary>
[Dependency(ServiceLifetime.Singleton, ReplaceServices = true), ExposeServices(typeof(IUserAgentProvider))]
public class UserAgentsMeProvider : IUserAgentProvider
{
    private const string UserAgentsMeCacheKey = "www_useragents_me";

    private readonly UserAgentHttpClient _userAgentHttpClient;
    private readonly IDistributedCache<UserAgentCacheItem> _distributedCache;

    public UserAgentsMeProvider(UserAgentHttpClient userAgentHttpClient,
        IDistributedCache<UserAgentCacheItem> cache)
    {
        _userAgentHttpClient = userAgentHttpClient;
        _distributedCache = cache;
    }

    /// <summary>
    /// 随机从User-Agent池中获取一个User-Agent
    /// </summary>
    /// <returns></returns>
    public async Task<string> GetOnesAsync()
    {
        Random random = new Random(Guid.NewGuid().GetHashCode());

        var cache = await _distributedCache.GetAsync(UserAgentsMeCacheKey);
        if (cache is { UaPools: { Count: > 0 } })
        {
            return cache.UaPools[random.Next(0, cache.UaPools.Count - 1)];
        }
        else
        {
            try
            {
                var result = await _userAgentHttpClient.GetUserAgentsAsync();
                if (result is { Count: > 0 })
                {
                    await _distributedCache.SetAsync(UserAgentsMeCacheKey, new UserAgentCacheItem
                    {
                        UaPools = result
                    }, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTimeOffset.Now.AddDays(1)
                    });
                    return result[random.Next(0, result.Count - 1)];
                }
                else
                {
                    return UserAgentPoolHelper.RandomGetOne();
                }
            }
            catch (Exception e)
            {
                return UserAgentPoolHelper.RandomGetOne();
            }
        }
    }
}