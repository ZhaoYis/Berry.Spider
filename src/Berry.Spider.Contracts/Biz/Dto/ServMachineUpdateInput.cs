using Berry.Spider.Core;

namespace Berry.Spider.Biz;

public class ServMachineUpdateInput
{
    /// <summary>
    /// 自定义业务ID
    /// </summary>
    public string BizNo { get; set; }

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
    /// 状态
    /// </summary>
    public MachineStatus Status { get; set; }

    /// <summary>
    /// 最后一次在线时间
    /// </summary>
    public DateTime? LastOnlineTime { get; set; }
}