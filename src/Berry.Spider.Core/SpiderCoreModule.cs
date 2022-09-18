using System.Reflection;
using Berry.Spider.Contracts;
using Berry.Spider.Proxy;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Caching;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.Modularity;
using Volo.Abp.TextTemplating.Scriban;

namespace Berry.Spider.Core;

[DependsOn(typeof(AbpCachingModule),
    typeof(AbpCachingStackExchangeRedisModule),
    typeof(AbpTextTemplatingScribanModule),
    //IP代理
    typeof(SpiderProxyModule))]
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
        //配置QuartzOptions
        context.Services.Configure<SpiderQuartzOptions>(configuration.GetSection(nameof(SpiderQuartzOptions)));
        //配置ExceptionlessOptions
        context.Services.Configure<ExceptionlessOptions>(configuration.GetSection(nameof(ExceptionlessOptions)));
        //配置TitleTemplateContentOptions
        context.Services.Configure<TitleTemplateContentOptions>(configuration.GetSection(nameof(TitleTemplateContentOptions)));
        //配置AbstractTemplateOptions
        context.Services.Configure<AbstractTemplateOptions>(configuration.GetSection(nameof(AbstractTemplateOptions)));

        //注入文本解析器
        context.Services.AddTransient<TouTiaoQuestionTextAnalysisProvider>();
        context.Services.AddTransient<BaiduRelatedSearchTextAnalysisProvider>();
        context.Services.AddTransient<SogouRelatedSearchTextAnalysisProvider>();
        context.Services.AddTransient<NormalTextAnalysisProvider>();

        //分布式缓存
        Configure<AbpDistributedCacheOptions>(options =>
        {
            options.KeyPrefix = "Berry:Spider:";
            options.GlobalCacheEntryOptions.SlidingExpiration = TimeSpan.FromMinutes(10);
            options.GlobalCacheEntryOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30);
        });
        //分布式缓存Redis
        Configure<RedisCacheOptions>(options =>
        {
            options.InstanceName = Assembly.GetExecutingAssembly().FullName;
        });

        return Task.CompletedTask;
    }
}