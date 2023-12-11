namespace Berry.Spider.RealTime;

public interface ISpiderAgentNotifyHub
{
    /// <summary>
    /// 向所有客户端发送消息
    /// </summary>
    /// <returns></returns>
    Task SendToAllAsync(SpiderAgentNotifyDto notify);
    
    /// <summary>
    /// 推送Agent客户端信息
    /// </summary>
    /// <returns></returns>
    Task PushAgentClientInfoAsync(AgentClientInfoDto agentClientInfo);
}