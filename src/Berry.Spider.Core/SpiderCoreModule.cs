using Berry.Spider.Contracts;
using Berry.Spider.FreeRedis;
using Berry.Spider.Proxy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;
using Volo.Abp.Caching;
using Volo.Abp.Modularity;
using Volo.Abp.TextTemplating.Scriban;

namespace Berry.Spider.Core;

[DependsOn(typeof(AbpCachingModule),
    typeof(AbpTextTemplatingScribanModule),
    //FreeRedis
    typeof(SpiderFreeRedisModule),
    //IP代理
    typeof(SpiderProxyModule))]
public class SpiderCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
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
        //配置MongoDBOptions
        context.Services.Configure<MongoDBOptions>(configuration.GetSection(nameof(MongoDBOptions)));
        //配置ConsulOptions
        context.Services.Configure<ConsulOptions>(configuration.GetSection(nameof(ConsulOptions)));
        //配置OpenAIOptions
        context.Services.Configure<OpenAIOptions>(configuration.GetSection(nameof(OpenAIOptions)));
        //配置ConsumerOptions
        context.Services.Configure<ConsumerOptions>(configuration.GetSection(nameof(ConsumerOptions)));

        //注入文本解析器
        context.Services.AddTransient<NormalTextAnalysisProvider>();

        //注册解析真实跳转的Url地址解析器
        context.Services.AddSingleton<NormalResolveJumpUrlProvider>();

        //对象池
        context.Services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
        context.Services.AddSingleton<IStringBuilderObjectPoolProvider, StringBuilderObjectPoolProvider>();

        //User-Agent
        context.Services.AddHttpClient<UserAgentHttpClient>();
    }
}