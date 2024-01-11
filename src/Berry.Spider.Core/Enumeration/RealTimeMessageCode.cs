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
    [Description("系统消息")] SYSTEM_MESSAGE = 200,

    /// <summary>
    /// 通知Agent节点开始部署应用
    /// </summary>
    [Description("通知Agent节点开始部署应用")] NOTIFY_AGENT_TO_START_DEPLOYING_APP = 1000,

    /// <summary>
    /// 通知Agent节点重启所有应用
    /// </summary>
    [Description("通知Agent节点重启所有应用")] NOTIFY_AGENT_TO_RESTART_APP = 1100
}