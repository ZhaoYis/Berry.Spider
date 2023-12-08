using Volo.Abp;
using Volo.Abp.AspNetCore.SignalR;
using Volo.Abp.Modularity;

namespace Berry.Spider.RealTime;

[DependsOn(
    typeof(AbpAspNetCoreSignalRModule),
    typeof(SpiderRealTimeAbstractionsModule)
)]
public class SpiderRealTimeModule : AbpModule
{
    public override async Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        await base.OnApplicationInitializationAsync(context);
    }
}