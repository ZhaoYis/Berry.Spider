using Volo.Abp;
using Volo.Abp.Modularity;

namespace Berry.Spider.RealTime;

public class SpiderRealTimeAbstractionsModule : AbpModule
{
    public override async Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        await base.OnApplicationInitializationAsync(context);
    }
}