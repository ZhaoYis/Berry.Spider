using Microsoft.AspNetCore.Http.Connections;
using Volo.Abp.AspNetCore.SignalR;
using Volo.Abp.Modularity;

namespace Berry.Spider.RealTime;

[DependsOn(
    typeof(AbpAspNetCoreSignalRModule),
    typeof(SpiderRealTimeAbstractionsModule)
)]
public class SpiderRealTimeModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpSignalROptions>(options =>
        {
            options.Hubs.AddOrUpdate(
                typeof(SpiderAppNotifyHub),
                config =>
                {
                    config.RoutePattern = "/signalr-hubs/spider/app-notify";
                    config.ConfigureActions.Add(hubOptions =>
                    {
                        hubOptions.Transports = HttpTransportType.WebSockets;
                    });
                }
            );
            
            options.Hubs.AddOrUpdate(
                typeof(SpiderMonitorNotifyHub),
                config =>
                {
                    config.RoutePattern = "/signalr-hubs/spider/monitor-notify";
                    config.ConfigureActions.Add(hubOptions =>
                    {
                        hubOptions.Transports = HttpTransportType.WebSockets;
                    });
                }
            );
        });
    }
}