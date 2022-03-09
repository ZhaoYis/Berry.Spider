using Berry.Spider.Core;
using Berry.Spider.Domain;
using Berry.Spider.Proxy;
using Volo.Abp.Application;
using Volo.Abp.EventBus.RabbitMq;
using Volo.Abp.Modularity;

namespace Berry.Spider.Baidu;

[DependsOn(
    typeof(SpiderCoreModule),
    typeof(SpiderProxyModule),
    typeof(BaiduSpiderContractsModule),
    typeof(SpiderDomainModule),
    typeof(AbpEventBusRabbitMqModule),
    typeof(AbpDddApplicationModule)
)]
public class BaiduSpiderModule : AbpModule
{
    public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
    {
        return base.ConfigureServicesAsync(context);
    }
}