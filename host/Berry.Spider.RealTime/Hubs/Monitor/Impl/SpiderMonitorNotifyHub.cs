using Berry.Spider.Core;
using Volo.Abp.AspNetCore.SignalR;

namespace Berry.Spider.RealTime;

/// <summary>
/// 爬虫监控服务Hub
/// </summary>
public class SpiderMonitorNotifyHub : AbpHub<ISpiderMonitorReceiveHub>, ISpiderMonitorNotifyHub
{
    /// <summary>
    /// 向所有客户端发送消息
    /// </summary>
    /// <returns></returns>
    public async Task SendToAllAsync(SpiderMonitorNotifyDto notify)
    {
        await Clients.All.ReceiveSystemMessageAsync(new ReceiveSystemMessageDto
        {
            Code = notify.Code,
            Data = notify.Data,
            Message = notify.Message
        });
    }

    /// <summary>
    /// 推送Agent客户端信息
    /// </summary>
    /// <returns></returns>
    public Task PushMonitorAgentClientInfoAsync(MonitorAgentClientInfoDto agentClientInfo)
    {
        //TODO
        return Task.CompletedTask;
    }

    /// <summary>
    /// Called when a new connection is established with the hub.
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();

        //向当前连接节点发送连接成功消息
        await Clients.Caller.ReceiveSystemMessageAsync(new ReceiveSystemMessageDto
        {
            Code = RealTimeMessageCode.CONNECTION_SUCCESSFUL,
            Data = Context.ConnectionId,
            Message = $"上线通知，您的用户编号为：{Context.ConnectionId}"
        });
    }
}