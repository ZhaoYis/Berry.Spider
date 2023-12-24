using Berry.Spider.Core;
using Volo.Abp.AspNetCore.SignalR;

namespace Berry.Spider.RealTime;

/// <summary>
/// 爬虫程序通知Hub
/// </summary>
[HubRoute("/signalr-hubs/spider/app-notify")]
public class SpiderAppNotifyHub : AbpHub<ISpiderAppReceiveHub>, ISpiderAppNotifyHub
{
    private readonly IServAgentIntegration _servAgentIntegration;

    public SpiderAppNotifyHub(IServAgentIntegration servAgentIntegration)
    {
        _servAgentIntegration = servAgentIntegration;
    }

    /// <summary>
    /// 向所有客户端发送消息
    /// </summary>
    /// <returns></returns>
    public async Task SendToAllAsync(SpiderAppNotifyDto notify)
    {
        await this.Clients.Groups(new[] { MachineGroupCode.App.GetName() }).ReceiveSystemMessageAsync(new ReceiveSystemMessageDto
        {
            Code = notify.Code,
            Data = notify.Data,
            Message = notify.Message
        });
    }

    /// <summary>
    /// 推送App客户端信息
    /// </summary>
    /// <returns></returns>
    public async Task PushAppClientInfoAsync(AppClientInfoDto appClientInfo)
    {
        Console.WriteLine(appClientInfo.Data.MachineName + "上线啦～");

        AppClientInfo clientInfo = appClientInfo.Data;
        ServMachineOnlineDto machineOnlineDto = new ServMachineOnlineDto
        {
            MachineName = clientInfo.MachineName,
            MachineCode = clientInfo.MachineCode,
            MachineIpAddr = clientInfo.MachineIpAddr,
            MachineMacAddr = clientInfo.MachineMacAddr,
            GroupCode = MachineGroupCode.App.GetName(),
            ConnectionId = Context.ConnectionId
        };
        var apiResp = await _servAgentIntegration.OnlineAsync(machineOnlineDto);
        if (apiResp.IsSuccessful)
        {
            //加到Agent组中
            await this.Groups.AddToGroupAsync(Context.ConnectionId, MachineGroupCode.App.GetName());
        }
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
        //加到App组中
        await this.Groups.AddToGroupAsync(Context.ConnectionId, MachineGroupCode.App.GetName());
    }

    /// <summary>
    /// Called when a connection with the hub is terminated.
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        ServMachineOfflineDto machineOfflineDto = new ServMachineOfflineDto
        {
            ConnectionId = Context.ConnectionId,
            GroupCode = MachineGroupCode.App.GetName()
        };
        var apiResp = await _servAgentIntegration.OfflineAsync(machineOfflineDto);
        
        //从组中移除Agent
        await this.Groups.RemoveFromGroupAsync(Context.ConnectionId, MachineGroupCode.App.GetName());
    }
}