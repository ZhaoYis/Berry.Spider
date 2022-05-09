using Berry.Spider.Contracts;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Caching;
using Volo.Abp.Modularity;

namespace Berry.Spider.Core;

[DependsOn(typeof(AbpCachingModule))]
public class SpiderCoreModule : AbpModule
{
    public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        //配置WebDriverOptions
        context.Services.Configure<WebDriverOptions>(configuration.GetSection(nameof(WebDriverOptions)));
        //配置ImageResourceOptions
        context.Services.Configure<ImageResourceOptions>(configuration.GetSection(nameof(ImageResourceOptions)));
        //配置SpiderOptions
        context.Services.Configure<SpiderOptions>(configuration.GetSection(nameof(SpiderOptions)));
        //配置HumanMachineVerificationOptions
        context.Services.Configure<HumanMachineVerificationOptions>(configuration.GetSection(nameof(HumanMachineVerificationOptions)));

        //注入文本解析器
        context.Services.AddTransient<TouTiaoQuestionTextAnalysisProvider>();
        context.Services.AddTransient<BaiduRelatedSearchTextAnalysisProvider>();
        
        //分布式缓存
        Configure<AbpDistributedCacheOptions>(options =>
        {
            options.KeyPrefix = "Berry:Spider:";
            options.GlobalCacheEntryOptions.SlidingExpiration = TimeSpan.FromMinutes(10);
            options.GlobalCacheEntryOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30);
        });
        
        return Task.CompletedTask;
    }
}