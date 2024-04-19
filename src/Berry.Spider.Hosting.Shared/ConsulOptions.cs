namespace Berry.Spider.Hosting.Shared;

public class ConsulOptions
{
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Consul主机地址
    /// </summary>
    public string Host { get; set; }

    /// <summary>
    /// 健康检查地址
    /// </summary>
    public string HealthCheckPath { get; set; }

    /// <summary>
    /// 应用信息
    /// </summary>
    public AppInfo App { get; set; }
}

public class AppInfo
{
    /// <summary>
    /// 应用名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 协议
    /// </summary>
    public string Scheme { get; set; }

    /// <summary>
    /// 应用主机地址
    /// </summary>
    public string Host { get; set; }

    /// <summary>
    /// 应用监听端口
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// 标签
    /// </summary>
    public string[] Tags { get; set; }
}