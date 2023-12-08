namespace Berry.Spider.Admin;

public interface ISpiderMonitorNotifyHub
{
    /// <summary>
    /// 向所有客户端发送消息
    /// </summary>
    /// <returns></returns>
    Task SendToAllAsync(SpiderMonitorNotifyDto notify);
}