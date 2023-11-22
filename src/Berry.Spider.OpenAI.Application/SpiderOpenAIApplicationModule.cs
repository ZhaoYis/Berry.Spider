using Berry.Spider.Core;
using Berry.Spider.Domain;
using Berry.Spider.OpenAI.Contracts;
using Volo.Abp.Application;
using Volo.Abp.Modularity;

namespace Berry.Spider.OpenAI.Application;

[DependsOn(
    typeof(SpiderCoreModule),
    typeof(SpiderOpenAIContractsModule),
    typeof(SpiderDomainModule),
    typeof(AbpDddApplicationModule),
    //OpenAI模块
    typeof(SpiderOpenAIModule)
)]
public class SpiderOpenAIApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        base.ConfigureServices(context);
    }
}