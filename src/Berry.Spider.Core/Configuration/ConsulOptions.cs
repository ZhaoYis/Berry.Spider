namespace Berry.Spider.Contracts;

public class ConsulOptions
{
    /// <summary>
    /// 是否启用Consul
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// 主机地址
    /// </summary>
    public string DiscoveryServerHostName { get; set; }

    /// <summary>
    /// 端口
    /// </summary>
    public int DiscoveryServerPort { get; set; } = 8500;
}