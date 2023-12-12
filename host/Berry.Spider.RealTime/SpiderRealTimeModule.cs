using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AspNetCore.SignalR;
using Volo.Abp.Modularity;

namespace Berry.Spider.RealTime;

[DependsOn(
    typeof(AbpAspNetCoreSignalRModule),
    typeof(SpiderRealTimeAbstractionsModule),
    typeof(SpiderRealTimeIntegrationModule)
)]
public class SpiderRealTimeModule : AbpModule
{
    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        //fix：https://github.com/abpframework/abp/issues/18415
        context.Services.Configure<AbpSignalROptions>(options =>
        {
            var hubs = options.Hubs.DistinctBy(x => x.HubType).ToList();
            options.Hubs.Clear();
            options.Hubs.AddRange(hubs);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        // Configure<AbpSignalROptions>(options =>
        // {
        //     options.Hubs.AddOrUpdate(
        //         typeof(SpiderAppNotifyHub),
        //         config =>
        //         {
        //             config.RoutePattern = "/signalr-hubs/spider/app-notify";
        //             config.ConfigureActions.Add(hubOptions => { hubOptions.Transports = HttpTransportType.WebSockets; });
        //         }
        //     );
        //
        //     options.Hubs.AddOrUpdate(
        //         typeof(SpiderAgentNotifyHub),
        //         config =>
        //         {
        //             config.RoutePattern = "/signalr-hubs/spider/agent-notify";
        //             config.ConfigureActions.Add(hubOptions => { hubOptions.Transports = HttpTransportType.WebSockets; });
        //         }
        //     );
        // });
    }
}