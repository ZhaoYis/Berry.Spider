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
        //注入头条爬虫提供者
        context.Services.AddTransient<TouTiaoSpider4QuestionProvider>();
        context.Services.AddTransient<TouTiaoSpider4InformationProvider>();

        return Task.CompletedTask;
    }
}