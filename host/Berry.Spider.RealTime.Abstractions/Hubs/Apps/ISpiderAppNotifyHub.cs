namespace Berry.Spider.RealTime;

public interface ISpiderAppNotifyHub
{
    /// <summary>
    /// 向所有客户端发送消息
    /// </summary>
    /// <returns></returns>
    Task SendToAllAsync(SpiderAppNotifyDto notify);
    
    /// <summary>
    /// 推送App客户端信息
    /// </summary>
    /// <returns></returns>
    Task PushAppClientInfoAsync(AppClientInfoDto appClientInfo);
}