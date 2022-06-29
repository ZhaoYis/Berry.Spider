using Berry.Spider.Contracts;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenQA.Selenium;

namespace Berry.Spider.Core;

public class HumanMachineVerificationInterceptorProvider : IHumanMachineVerificationInterceptorProvider
{
    private const string HumanMachineVerificationInterceptorCacheKey = "HumanMachineVerificationInterceptor";

    private ILogger<HumanMachineVerificationInterceptorProvider> Logger { get; }
    private IDistributedCache Cache { get; }
    private IOptionsSnapshot<HumanMachineVerificationOptions> Options { get; }

    public HumanMachineVerificationInterceptorProvider(ILogger<HumanMachineVerificationInterceptorProvider> logger,
        IDistributedCache cache,
        IOptionsSnapshot<HumanMachineVerificationOptions> options)
    {
        this.Logger = logger;
        this.Cache = cache;
        this.Options = options;
    }

    public async Task InvokeAsync(IWebDriver webDriver)
    {
        string url = webDriver.Url;

        if (this.Options.Value.BlackHosts.Contains(url))
        {
            this.Logger.LogInformation($"命中了人机验证，只有等一会儿咯~");
            //等一会儿吧~
            string cacheKey = string.Format("{0}:{1}", HumanMachineVerificationInterceptorCacheKey, Environment.MachineName);
            await this.Cache.SetStringAsync(cacheKey, url,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(this.Options.Value.LockExpiration)
                });
        }

        await Task.CompletedTask;
    }

    public async Task<bool> IsLockedAsync()
    {
        string cacheKey = string.Format("{0}:{1}", HumanMachineVerificationInterceptorCacheKey, Environment.MachineName);
        string val = await this.Cache.GetStringAsync(cacheKey);
        return !string.IsNullOrEmpty(val);
    }
}