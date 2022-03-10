using Berry.Spider.Core;
using Berry.Spider.Domain;
using Berry.Spider.Proxy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.Application;
using Volo.Abp.EventBus.RabbitMq;
using Volo.Abp.Modularity;

namespace Berry.Spider.TouTiao;

[DependsOn(
    typeof(SpiderCoreModule),
    typeof(SpiderProxyModule),
    typeof(TouTiaoSpiderContractsModule),
    typeof(SpiderDomainModule),
    typeof(AbpEventBusRabbitMqModule),
    typeof(AbpDddApplicationModule)
)]
public class TouTiaoSpiderModule : AbpModule
{
    public override Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        var logger = context.ServiceProvider.GetRequiredService<ILogger<TouTiaoSpiderModule>>();
        var configuration = context.ServiceProvider.GetRequiredService<IConfiguration>();

        var hostEnvironment = context.ServiceProvider.GetRequiredService<IHostEnvironment>();
        logger.LogInformation($"EnvironmentName => {hostEnvironment.EnvironmentName}");

        return Task.CompletedTask;
    }

    public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
    {
        //ע��ͷ�������ṩ��
        context.Services.AddTransient<TouTiaoSpider4QuestionProvider>();
        context.Services.AddTransient<TouTiaoSpider4InformationProvider>();

        return Task.CompletedTask;
    }
}