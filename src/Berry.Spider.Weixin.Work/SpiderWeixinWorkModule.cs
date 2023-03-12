using Volo.Abp.Modularity;

namespace Berry.Spider.Weixin.Work;

[DependsOn(typeof(SpiderWeixinModule))]
public class SpiderWeixinWorkModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        base.ConfigureServices(context);
    }
}