using Berry.Spider.Domain.Shared;
using Volo.Abp.Modularity;

namespace Berry.Spider.TouTiao.Contracts;

[DependsOn(
    typeof(SpiderDomainSharedModule)
)]
public class TouTiaoSpiderContractsModule : AbpModule
{
    public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
    {
        return base.ConfigureServicesAsync(context);
    }
}