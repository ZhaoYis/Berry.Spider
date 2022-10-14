using Berry.Spider.Abstractions;
using Berry.Spider.Core;
using Berry.Spider.Domain;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.Modularity;

namespace Berry.Spider.TouTiao;

[DependsOn(
    typeof(AbpDddApplicationModule),
    typeof(SpiderCoreModule),
    typeof(SpiderDomainModule),
    typeof(SpiderAbstractionsModule),
    typeof(TouTiaoSpiderContractsModule)
)]
public class TouTiaoSpiderModule : AbpModule
{
    public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
    {
        context.Services.AddTransient<TouTiaoSpider4QuestionProvider>();
        context.Services.AddTransient<TouTiaoSpider4InformationProvider>();
        context.Services.AddTransient<TouTiaoSpider4InformationCompositionProvider>();

        return Task.CompletedTask;
    }
}