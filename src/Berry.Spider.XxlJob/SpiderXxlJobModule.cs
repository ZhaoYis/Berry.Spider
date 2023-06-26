using DotXxlJob.Core;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace Berry.Spider.XxlJob;

public class SpiderXxlJobModule : AbpModule
{
    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();

        //启用XxlExecutor
        app.UseXxlJobExecutor();
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        context.Services.AddXxlJobExecutor(configuration);
        //自动注册
        context.Services.AddAutoRegistry();
    }
}