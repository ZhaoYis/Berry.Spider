namespace Berry.Spider.RealTime;

public interface ISpiderMonitorNotifyHub
{
    /// <summary>
    /// 向所有客户端发送消息
    /// </summary>
    /// <returns></returns>
    Task SendToAllAsync(SpiderMonitorNotifyDto notify);
    
    /// <summary>
    /// 推送Agent客户端信息
    /// </summary>
    /// <returns></returns>
    Task PushMonitorAgentClientInfoAsync(MonitorAgentClientInfoDto agentClientInfo);
}