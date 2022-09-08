using Volo.Abp.Modularity;

namespace Berry.Spider.Proxy.Abstractions;

public class SpiderProxyAbstractionsModule : AbpModule
{
    public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
    {
        return Task.CompletedTask;
    }
}