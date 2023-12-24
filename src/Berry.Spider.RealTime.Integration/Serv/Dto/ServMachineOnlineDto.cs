namespace Berry.Spider.RealTime;

public class ServMachineOnlineDto
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
    /// ConnectionId
    /// </summary>
    public string ConnectionId { get; set; }

    /// <summary>
    /// 分组编码
    /// </summary>
    public string GroupCode { get; set; }
}