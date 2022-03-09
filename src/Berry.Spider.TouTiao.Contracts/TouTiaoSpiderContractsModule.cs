using Berry.Spider.Domain.Shared;
using Volo.Abp.Application;
using Volo.Abp.Modularity;

namespace Berry.Spider.TouTiao;

[DependsOn(
    typeof(SpiderDomainSharedModule),
    typeof(AbpDddApplicationContractsModule)
)]
public class TouTiaoSpiderContractsModule : AbpModule
{
    public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
    {
        return base.ConfigureServicesAsync(context);
    }
}