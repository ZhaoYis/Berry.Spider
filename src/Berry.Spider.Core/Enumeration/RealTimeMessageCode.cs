using System.ComponentModel;

namespace Berry.Spider.Core;

public enum RealTimeMessageCode
{
    /// <summary>
    /// 连接成功
    /// </summary>
    [Description("连接成功")] CONNECTION_SUCCESSFUL = 100,

    /// <summary>
    /// 系统消息（用于通知、公告等）
    /// </summary>
    [Description("系统消息")] SYSTEM_MESSAGE = 200
}