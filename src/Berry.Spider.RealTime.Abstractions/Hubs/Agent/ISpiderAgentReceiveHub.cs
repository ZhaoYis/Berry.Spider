namespace Berry.Spider.RealTime;

public interface ISpiderAgentReceiveHub : ISystemReceiveHub
{
    /// <summary>
    /// 终端接收服务器消息
    /// </summary>
    /// <returns></returns>
    Task ReceiveMessageAsync(SpiderAgentReceiveDto receive);
}