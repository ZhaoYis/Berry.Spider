using Berry.Spider.Contracts;
using Berry.Spider.Core;
using Berry.Spider.Domain;
using Berry.Spider.Mmonly.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Application;
using Volo.Abp.BackgroundJobs.Quartz;
using Volo.Abp.Modularity;
using Volo.Abp.Quartz;

namespace Berry.Spider.Mmonly;

[DependsOn(
    typeof(SpiderCoreModule),
    typeof(MmonlySpiderContractsModule),
    typeof(SpiderDomainModule),
    typeof(AbpDddApplicationModule),
    typeof(AbpBackgroundJobsQuartzModule)
)]
public class MmonlySpiderModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        SpiderQuartzOptions quartzOptions =
            configuration.GetSection(nameof(SpiderQuartzOptions)).Get<SpiderQuartzOptions>();

        PreConfigure<AbpQuartzOptions>(options =>
        {
            // options.Properties = new NameValueCollection
            // {
            //     ["quartz.jobStore.dataSource"] = "berry_spider_quartz",
            //     ["quartz.jobStore.type"] = "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz",
            //     ["quartz.jobStore.tablePrefix"] = "QRTZ_",
            //     ["quartz.serializer.type"] = "json",
            //     ["quartz.dataSource.berry_spider_quartz.connectionString"] = quartzOptions.ConnectionString,
            //     ["quartz.dataSource.berry_spider_quartz.provider"] = "MySql",
            //     ["quartz.jobStore.driverDelegateType"] = "Quartz.Impl.AdoJobStore.MySQLDelegate, Quartz",
            // };

            options.Configurator = configure =>
            {
                // configure.UsePersistentStore(storeOptions =>
                // {
                //     storeOptions.UseProperties = true;
                //     storeOptions.UseClustering(c =>
                //     {
                //         c.CheckinMisfireThreshold = TimeSpan.FromSeconds(20);
                //         c.CheckinInterval = TimeSpan.FromSeconds(10);
                //     });
                // });
                configure.UseInMemoryStore();
                configure.MaxBatchSize = 100;
                configure.UseDefaultThreadPool(tp => { tp.MaxConcurrency = 10; });
            };
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpBackgroundJobQuartzOptions>(options =>
        {
            options.RetryCount = 1;
            options.RetryIntervalMillisecond = 1000;
        });
    }
}