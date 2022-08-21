using Berry.Spider.Baidu;
using Berry.Spider.Contracts;
using Berry.Spider.EntityFrameworkCore;
using Berry.Spider.TouTiao;
using Exceptionless;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.EventBus.RabbitMq;
using Volo.Abp.Modularity;

namespace Berry.Spider.Consumers;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpEventBusRabbitMqModule),
    typeof(SpiderEntityFrameworkCoreModule),
    //今日头条模块
    typeof(TouTiaoSpiderModule),
    //百度模块
    typeof(BaiduSpiderModule)
)]
public class SpiderConsumersModule : AbpModule
{
    public override Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        var logger = context.ServiceProvider.GetRequiredService<ILogger<SpiderConsumersModule>>();
        var configuration = context.ServiceProvider.GetRequiredService<IConfiguration>();

        var hostEnvironment = context.ServiceProvider.GetRequiredService<IHostEnvironment>();
        logger.LogInformation($"EnvironmentName => {hostEnvironment.EnvironmentName}");

        //集成Exceptionless
        ExceptionlessOptions options = configuration.GetSection(nameof(ExceptionlessOptions)).Get<ExceptionlessOptions>();
        if (options.IsEnable && !string.IsNullOrEmpty(options.ApiKey))
        {
            ExceptionlessClient.Default.Startup(options.ApiKey);
        }

        return Task.CompletedTask;
    }
}