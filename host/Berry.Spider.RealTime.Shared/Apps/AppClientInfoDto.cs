using Berry.Spider.Core;

namespace Berry.Spider.RealTime;

[InvokeMethodName("PushAppClientInfoAsync")]
public class AppClientInfoDto : NotifyMessageBase<AppClientInfo>
{
    public AppClientInfoDto()
    {
        this.Code = RealTimeMessageCode.CONNECTION_SUCCESSFUL;
    }
}

public class AppClientInfo
{
    /// <summary>
    /// 机器名
    /// </summary>
    public string MachineName { get; set; }

    /// <summary>
    /// 机器编码
    /// </summary>
    public string MachineCode { get; set; }

    /// <summary>
    /// 机器IP地址
    /// </summary>
    public string MachineIpAddr { get; set; }

    /// <summary>
    /// 机器MAC地址
    /// </summary>
    public string MachineMacAddr { get; set; }

    /// <summary>
    /// 连接ID
    /// </summary>
    public string? ConnectionId { get; set; }
}