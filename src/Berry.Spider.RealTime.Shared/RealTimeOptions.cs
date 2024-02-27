namespace Berry.Spider.RealTime;

public class RealTimeOptions
{
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// Agent终节点
    /// </summary>
    public string AgentEndpointUrl { get; set; }

    /// <summary>
    /// App终节点
    /// </summary>
    public string AppEndpointUrl { get; set; }
}