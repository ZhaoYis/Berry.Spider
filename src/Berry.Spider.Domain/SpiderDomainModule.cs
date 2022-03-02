using Berry.Spider.Domain.Shared;
using Volo.Abp.Modularity;

namespace Berry.Spider.Domain;

[DependsOn(
    typeof(SpiderDomainSharedModule)
)]
public class SpiderDomainModule : AbpModule
{
    public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
    {
        return Task.CompletedTask;
    }
}