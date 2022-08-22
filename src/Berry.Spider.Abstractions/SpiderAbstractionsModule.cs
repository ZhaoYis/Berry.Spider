using Volo.Abp.Modularity;

namespace Berry.Spider.Abstractions;

[DependsOn(
)]
public class SpiderAbstractionsModule : AbpModule
{
    public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
    {
        return Task.CompletedTask;
    }
}