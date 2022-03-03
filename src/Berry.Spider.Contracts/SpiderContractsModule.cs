using Berry.Spider.Domain.Shared;
using Volo.Abp.Modularity;

namespace Berry.Spider.Contracts;

[DependsOn(
    typeof(SpiderDomainSharedModule)
)]
public class SpiderContractsModule : AbpModule
{
    public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
    {
        return base.ConfigureServicesAsync(context);
    }
}