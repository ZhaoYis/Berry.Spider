using System.Collections.Specialized;
using Berry.Spider.Contracts;
using Berry.Spider.Core;
using Berry.Spider.Domain;
using Berry.Spider.Mmonly.Contracts;
using Berry.Spider.Proxy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Volo.Abp.Application;
using Volo.Abp.BackgroundJobs.Quartz;
using Volo.Abp.Modularity;
using Volo.Abp.Quartz;

namespace Berry.Spider.Mmonly;

[DependsOn(
    typeof(SpiderCoreModule),
    typeof(SpiderProxyModule),
    typeof(MmonlySpiderContractsModule),
    typeof(SpiderDomainModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpBackgroundJobsQuartzModule)
)]
public class MmonlySpiderModule: AbpModule
{public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        SpiderQuartzOptions quartzOptions = configuration.GetSection(nameof(SpiderQuartzOptions)).Get<SpiderQuartzOptions>();

        PreConfigure<AbpQuartzOptions>(options =>
        {
            options.Properties = new NameValueCollection
            {
                ["quartz.jobStore.dataSource"] = "berry_spider_quartz",
                ["quartz.jobStore.type"] = "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz",
                ["quartz.jobStore.tablePrefix"] = "QRTZ_",
                ["quartz.serializer.type"] = "json",
                ["quartz.dataSource.berry_spider_quartz.connectionString"] = quartzOptions.ConnectionString,
                ["quartz.dataSource.berry_spider_quartz.provider"] = "MySql",
                ["quartz.jobStore.driverDelegateType"] = "Quartz.Impl.AdoJobStore.MySQLDelegate, Quartz",
            };
        });
    }
    
    public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
    {
        Configure<AbpBackgroundJobQuartzOptions>(options =>
        {
            options.RetryCount = 5;
            options.RetryIntervalMillisecond = 5 * 1000;
        });
        
        return Task.CompletedTask;
    }
}