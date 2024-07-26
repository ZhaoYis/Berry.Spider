﻿using Berry.Spider.EntityFrameworkCore;
using Berry.Spider.NaiPan;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Berry.Spider.AI.TextGeneration;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(SpiderEntityFrameworkCoreModule),
    typeof(SpiderNaiPanModule)
)]
public class AITextGenerationModule : AbpModule
{
    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var logger = context.ServiceProvider.GetRequiredService<ILogger<AITextGenerationModule>>();
        var configuration = context.ServiceProvider.GetRequiredService<IConfiguration>();

        var hostEnvironment = context.ServiceProvider.GetRequiredService<IHostEnvironment>();
        logger.LogInformation($"EnvironmentName => {hostEnvironment.EnvironmentName}");
    }
}