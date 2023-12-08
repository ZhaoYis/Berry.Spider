using Volo.Abp.AspNetCore.SignalR;

namespace Berry.Spider.Admin;

/// <summary>
/// 爬虫监控服务Hub
/// </summary>
[HubRoute("/signalr-hubs/spider-monitor-notify")]
public class SpiderMonitorNotifyHub : AbpHub<ISpiderMonitorReceiveHub>, ISpiderMonitorNotifyHub
{
    /// <summary>
    /// 向所有客户端发送消息
    /// </summary>
    /// <returns></returns>
    public async Task SendToAllAsync(SpiderMonitorNotifyDto notify)
    {
        await Clients.All.ReceiveMessageAsync(new SpiderMonitorReceiveDto());
    }
}