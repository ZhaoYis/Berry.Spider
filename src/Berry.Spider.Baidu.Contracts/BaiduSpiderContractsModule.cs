using Berry.Spider.Domain.Shared;
using Volo.Abp.Application;
using Volo.Abp.Modularity;

namespace Berry.Spider.Baidu;

[DependsOn(
    typeof(SpiderDomainSharedModule),
    typeof(AbpDddApplicationContractsModule)
)]
public class BaiduSpiderContractsModule : AbpModule
{
    public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
    {
        return base.ConfigureServicesAsync(context);
    }
}