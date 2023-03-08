using Berry.Spider.Domain.Shared;
using Volo.Abp.Application;
using Volo.Abp.Modularity;

namespace Berry.Spider.OpenAI.Contracts;

[DependsOn(
    typeof(SpiderDomainSharedModule),
    typeof(AbpDddApplicationContractsModule)
)]
public class SpiderOpenAIContractsModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        base.ConfigureServices(context);
    }
}