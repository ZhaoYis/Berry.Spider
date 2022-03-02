using Volo.Abp.Modularity;

namespace Berry.Spider.Domain.Shared;

public class SpiderDomainSharedModule : AbpModule
{
    public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
    {
        return Task.CompletedTask;
    }
}