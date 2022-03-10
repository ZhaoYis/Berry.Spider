using Berry.Spider.Domain;
using Volo.Abp.Application;
using Volo.Abp.Modularity;

namespace Berry.Spider.Application;

[DependsOn(
    typeof(SpiderDomainModule),
    typeof(AbpDddApplicationModule),
    typeof(SpiderContractsModule))]
public class SpiderApplicationModule : AbpModule
{
    public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
    {
        return base.ConfigureServicesAsync(context);
    }
}