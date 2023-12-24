using Volo.Abp.Modularity;

namespace Berry.Spider.RealTime;

[DependsOn(typeof(SpiderRealTimeSharedModule))]
public class SpiderRealTimeAbstractionsModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        base.ConfigureServices(context);
    }
}