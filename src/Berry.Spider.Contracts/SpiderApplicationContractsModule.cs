using Berry.Spider.Domain.Shared;
using Volo.Abp.Application;
using Volo.Abp.Modularity;

namespace Berry.Spider;

[DependsOn(
    typeof(SpiderDomainSharedModule),
    typeof(AbpDddApplicationContractsModule)
)]
public class SpiderApplicationContractsModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        base.ConfigureServices(context);
    }
}