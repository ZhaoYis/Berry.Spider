using Berry.Spider.Domain.Shared;
using Volo.Abp.Modularity;

namespace Berry.Spider.Domain;

[DependsOn(
    typeof(SpiderDomainSharedModule)
)]
public class SpiderDomainModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        base.ConfigureServices(context);
    }
}