using Volo.Abp.AspNetCore.SignalR;

namespace Berry.Spider.RealTime;

/// <summary>
/// 爬虫程序通知Hub
/// </summary>
public class SpiderAppNotifyHub : AbpHub<ISpiderAppReceiveHub>, ISpiderAppNotifyHub
{
    /// <summary>
    /// 向所有客户端发送消息
    /// </summary>
    /// <returns></returns>
    public async Task SendToAllAsync(SpiderAppNotifyDto notify)
    {
        await Clients.All.ReceiveMessageAsync(new SpiderAppReceiveDto());
    }

    /// <summary>
    /// Called when a new connection is established with the hub.
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        await Clients.All.ReceiveSystemMessageAsync(new ReceiveSystemMessageDto
        {
            Message = "hello，" + this.Clock.Now.ToString("s")
        });
    }

    /// <summary>
    /// Called when a connection with the hub is terminated.
    /// </summary>
    public override Task OnDisconnectedAsync(Exception? exception)
    {
        return base.OnDisconnectedAsync(exception);
    }
}