using Berry.Spider.Core;

namespace Berry.Spider.RealTime;

[InvokeMethodName("PushAgentClientInfoAsync")]
public class AgentClientInfoDto : NotifyMessageBase<AgentClientInfo>
{
    public AgentClientInfoDto()
    {
        this.Code = RealTimeMessageCode.CONNECTION_SUCCESSFUL;
    }
}

public class AgentClientInfo
{
    /// <summary>
    /// 机器名
    /// </summary>
    public string MachineName { get; set; }

    /// <summary>
    /// 连接ID
    /// </summary>
    public string? ConnectionId { get; set; }
}