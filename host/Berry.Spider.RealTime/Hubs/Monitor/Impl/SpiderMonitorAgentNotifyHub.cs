using Berry.Spider.Core;
using Volo.Abp.AspNetCore.SignalR;

namespace Berry.Spider.RealTime;

/// <summary>
/// 爬虫监控Agent服务Hub
/// </summary>
public class SpiderMonitorAgentNotifyHub : AbpHub<ISpiderMonitorReceiveHub>, ISpiderMonitorNotifyHub
{
    private const string GroupName = "Agent";

    /// <summary>
    /// 向所有客户端发送消息
    /// </summary>
    /// <returns></returns>
    public async Task SendToAllAsync(SpiderMonitorNotifyDto notify)
    {
        await this.Clients.Groups(new[] { GroupName }).ReceiveSystemMessageAsync(new ReceiveSystemMessageDto
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
    public async Task PushMonitorAgentClientInfoAsync(MonitorAgentClientInfoDto agentClientInfo)
    {
        //TODO：机器上线
        Console.WriteLine(agentClientInfo.Data.MachineName + "上线啦～");

        //加到Agent组中
        await this.Groups.AddToGroupAsync(Context.ConnectionId, GroupName);
    }

    /// <summary>
    /// Called when a new connection is established with the hub.
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        //向当前连接节点发送连接成功消息
        await this.Clients.Caller.ReceiveSystemMessageAsync(new ReceiveSystemMessageDto
        {
            Code = RealTimeMessageCode.CONNECTION_SUCCESSFUL,
            Data = Context.ConnectionId,
            Message = $"上线通知，您的用户编号为：{Context.ConnectionId}"
        });
    }

    /// <summary>
    /// Called when a connection with the hub is terminated.
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        //TODO：机器下线

        //从组中移除Agent
        await this.Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName);
    }
}