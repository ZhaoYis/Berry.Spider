namespace Berry.Spider.RealTime;

public interface ISpiderAppReceiveHub : ISystemReceiveHub
{
    /// <summary>
    /// 终端接收服务器消息
    /// </summary>
    /// <returns></returns>
    Task ReceiveMessageAsync(SpiderAppReceiveDto receive);
}