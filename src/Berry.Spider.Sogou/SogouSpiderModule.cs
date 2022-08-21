using Berry.Spider.Core;
using Berry.Spider.Domain;
using Berry.Spider.Proxy;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.Modularity;

namespace Berry.Spider.Sogou;

[DependsOn(
    typeof(SpiderCoreModule),
    typeof(SpiderProxyModule),
    typeof(SogouSpiderContractsModule),
    typeof(SpiderDomainModule),
    typeof(AbpDddApplicationModule)
)]
public class SogouSpiderModule : AbpModule
{
    public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
    {
        //注入搜狗爬虫提供者
        context.Services.AddTransient<SogouSpider4RelatedSearchProvider>();

        return Task.CompletedTask;
    }
}