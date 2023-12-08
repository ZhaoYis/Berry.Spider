namespace Berry.Spider.Admin;

public interface ISpiderAppNotifyHub
{
    /// <summary>
    /// 向所有客户端发送消息
    /// </summary>
    /// <returns></returns>
    Task SendToAllAsync(SpiderAppNotifyDto notify);
}