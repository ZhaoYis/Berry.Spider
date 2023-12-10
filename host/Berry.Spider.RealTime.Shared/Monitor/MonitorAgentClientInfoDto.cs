using Berry.Spider.Core;

namespace Berry.Spider.RealTime;

[InvokeMethodName("PushMonitorAgentClientInfoAsync")]
public class MonitorAgentClientInfoDto : NotifyMessageBase<MonitorClientInfo>
{
}

public class MonitorClientInfo
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