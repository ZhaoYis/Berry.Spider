using Berry.Spider.Contracts;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.Caching;

namespace Berry.Spider.Core;

public class HumanMachineVerificationInterceptorProvider : IHumanMachineVerificationInterceptorProvider
{
    private ILogger<HumanMachineVerificationInterceptorProvider> Logger { get; }
    private IDistributedCache<HumanMachineVerificationInterceptorCacheItem> Cache { get; }
    private HumanMachineVerificationOptions Options { get; }

    public HumanMachineVerificationInterceptorProvider(ILogger<HumanMachineVerificationInterceptorProvider> logger,
        IDistributedCache<HumanMachineVerificationInterceptorCacheItem> cache,
        IOptionsSnapshot<HumanMachineVerificationOptions> options)
    {
        this.Logger = logger;
        this.Cache = cache;
        this.Options = options.Value;
    }

    public async Task<bool> LockedAsync(string sourcePage, string lockedPage)
    {
        if (!UrlHelper.IsUrl(sourcePage)) return false;
        string sourceHost = UrlHelper.GetHostString(sourcePage);

        foreach (string blackHost in this.Options.BlackHosts)
        {
            if (!lockedPage.Contains(blackHost)) continue;
            this.Logger.LogInformation($"命中了人机验证，只有等一会儿咯~");

            string cacheKey = $"{DnsHelper.GetHostName()}:{sourceHost}";
            await this.Cache.SetAsync(cacheKey, new HumanMachineVerificationInterceptorCacheItem
                {
                    LockPageUrl = lockedPage
                }
                , new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(this.Options.LockExpiration)
                });
            return true;
        }

        return false;
    }

    public async Task<bool> IsLockedAsync(string sourcePage)
    {
        if (!UrlHelper.IsUrl(sourcePage)) return false;
        string sourceHost = UrlHelper.GetHostString(sourcePage);

        string cacheKey = $"{DnsHelper.GetHostName()}:{sourceHost}";
        var cacheItem = await this.Cache.GetAsync(cacheKey);
        return cacheItem is not null && !string.IsNullOrEmpty(cacheItem.LockPageUrl);
    }

    [CacheName("HMVI")]
    public class HumanMachineVerificationInterceptorCacheItem
    {
        /// <summary>
        /// 触发人机验证的页面地址
        /// </summary>
        public string LockPageUrl { get; set; }
    }
}