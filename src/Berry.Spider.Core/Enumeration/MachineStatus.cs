using System.ComponentModel;

namespace Berry.Spider.Core;

/// <summary>
/// 机器状态
/// </summary>
public enum MachineStatus
{
    /// <summary>
    /// 未知
    /// </summary>
    [Description("未知")] None = 0,

    /// <summary>
    /// 在线
    /// </summary>
    [Description("在线")] Online = 10,

    /// <summary>
    /// 离线
    /// </summary>
    [Description("离线")] Offline = 20
}