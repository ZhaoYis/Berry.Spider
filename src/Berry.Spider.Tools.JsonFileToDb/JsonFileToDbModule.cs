using Berry.Spider.Core;
using Berry.Spider.Domain;
using Berry.Spider.EntityFrameworkCore;
using Berry.Spider.Segmenter.JiebaNet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Berry.Spider.Tools.JsonFileToDb;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(SpiderEntityFrameworkCoreModule),
    typeof(SpiderSegmenterJiebaNetModule),
    typeof(SpiderCoreModule),
    typeof(SpiderDomainModule)
)]
public class JsonFileToDbModule : AbpModule
{
    public override Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        var logger = context.ServiceProvider.GetRequiredService<ILogger<JsonFileToDbModule>>();
        var configuration = context.ServiceProvider.GetRequiredService<IConfiguration>();

        var hostEnvironment = context.ServiceProvider.GetRequiredService<IHostEnvironment>();
        logger.LogInformation($"EnvironmentName => {hostEnvironment.EnvironmentName}");

        return Task.CompletedTask;
    }
}