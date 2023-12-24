using Berry.Spider.FreeRedis;
using Berry.Spider.RealTime;
using Berry.Spider.Weixin.Work;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.BackgroundWorkers.Quartz;
using Volo.Abp.Modularity;

namespace Berry.Spider.Tools.ServDetector;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpBackgroundWorkersQuartzModule),
    //FreeRedis
    typeof(SpiderFreeRedisModule),
    typeof(SpiderWeixinWorkModule),
    typeof(SpiderRealTimeAbstractionsModule)
)]
public class SpiderServDetectorModule : AbpModule
{
    public override async Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        //注册服务
        // await context.AddBackgroundWorkerAsync<ServLifetimeCheckerWorker>();
    }
}