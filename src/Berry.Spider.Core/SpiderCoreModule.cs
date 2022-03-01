using Volo.Abp;
using Volo.Abp.Modularity;

namespace Berry.Spider.Core;

public class SpiderCoreModule : AbpModule
{
    public override Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        return base.OnApplicationInitializationAsync(context);
    }
}