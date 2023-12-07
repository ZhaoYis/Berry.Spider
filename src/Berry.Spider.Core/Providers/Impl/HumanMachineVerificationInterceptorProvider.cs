using System.Net;
using Berry.Spider.Contracts;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenQA.Selenium;
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

    public async Task<bool> LockedAsync(IWebDriver webDriver)
    {
        string url = webDriver.Url;
        foreach (string blackHost in this.Options.BlackHosts)
        {
            if (url.Contains(blackHost))
            {
                this.Logger.LogInformation($"命中了人机验证，只有等一会儿咯~");
                //等一会儿吧~
                string cacheKey = string.Format("{0}:{1}", "TODO：解析url", Dns.GetHostName());
                await this.Cache.SetAsync(cacheKey, new HumanMachineVerificationInterceptorCacheItem
                    {
                        LockPageUrl = url
                    }
                    , new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(this.Options.LockExpiration)
                    });
                return true;
            }
        }

        return false;
    }

    public async Task<bool> IsLockedAsync()
    {
        string cacheKey = string.Format("{0}:{1}", "TODO", Dns.GetHostName());
        var val = await this.Cache.GetAsync(cacheKey);
        return val != null && !string.IsNullOrEmpty(val.LockPageUrl);
    }
}

[CacheName("HMVI")]
public class HumanMachineVerificationInterceptorCacheItem
{
    public string LockPageUrl { get; set; }
}