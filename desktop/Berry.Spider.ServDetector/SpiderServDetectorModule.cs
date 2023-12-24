using Berry.Spider.FreeRedis;
using Berry.Spider.RealTime;
using Berry.Spider.Weixin.Work;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundWorkers.Quartz;
using Volo.Abp.Modularity;

namespace Berry.Spider.ServDetector;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpBackgroundWorkersQuartzModule),
    typeof(SpiderFreeRedisModule),
    typeof(SpiderWeixinWorkModule),
    typeof(SpiderRealTimeAbstractionsModule)
)]
public class SpiderServDetectorModule : AbpModule
{
    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        //注册服务
        // await context.AddBackgroundWorkerAsync<ServLifetimeCheckerWorker>();
    }
}