namespace Berry.Spider.RealTime;

public interface ISpiderMonitorReceiveHub : ISystemReceiveHub
{
    /// <summary>
    /// 终端接收服务器消息
    /// </summary>
    /// <returns></returns>
    Task ReceiveMessageAsync(SpiderMonitorReceiveDto receive);
}