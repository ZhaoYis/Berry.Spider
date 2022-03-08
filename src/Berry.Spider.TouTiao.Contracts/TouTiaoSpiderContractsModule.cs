using Berry.Spider.Contracts;
using Volo.Abp.Application;
using Volo.Abp.Modularity;

namespace Berry.Spider.TouTiao.Contracts;

[DependsOn(
    typeof(SpiderContractsModule),
    typeof(AbpDddApplicationContractsModule)
)]
public class TouTiaoSpiderContractsModule : AbpModule
{
    public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
    {
        return base.ConfigureServicesAsync(context);
    }
}