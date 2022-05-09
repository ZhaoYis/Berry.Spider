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
    private HumanMachineVerificationOptions Options { get; }

    public HumanMachineVerificationInterceptorProvider(ILogger<HumanMachineVerificationInterceptorProvider> logger,
        IDistributedCache cache,
        IOptions<HumanMachineVerificationOptions> options)
    {
        this.Logger = logger;
        this.Cache = cache;
        this.Options = options.Value;
    }

    public async Task InvokeAsync(IWebDriver webDriver)
    {
        string url = webDriver.Url;

        if (this.Options.BlackHosts.Contains(url))
        {
            this.Logger.LogInformation($"命中了人机验证，只有等一会儿咯~");
            //等一会儿吧~
            await this.Cache.SetStringAsync(HumanMachineVerificationInterceptorCacheKey, url, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(this.Options.LockExpiration)
            });
        }

        await Task.CompletedTask;
    }

    public async Task<bool> IsLockedAsync()
    {
        string val = await this.Cache.GetStringAsync(HumanMachineVerificationInterceptorCacheKey);
        return !string.IsNullOrEmpty(val);
    }
}