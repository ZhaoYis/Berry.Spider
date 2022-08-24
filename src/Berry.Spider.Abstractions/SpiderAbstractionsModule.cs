using Volo.Abp.Modularity;

namespace Berry.Spider.Abstractions;

public class SpiderAbstractionsModule : AbpModule
{
    public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
    {
        return Task.CompletedTask;
    }
}