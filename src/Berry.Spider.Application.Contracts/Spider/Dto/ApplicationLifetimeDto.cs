namespace Berry.Spider;

public class ApplicationLifetimeDto
{
    /// <summary>
    /// 服务是否可用
    /// </summary>
    public bool AreYouOk { get; set; }

    /// <summary>
    /// 机器名称
    /// </summary>
    public string MachineName { get; set; }

    /// <summary>
    /// 进程ID
    /// </summary>
    public int ProcessId { get; set; }

    /// <summary>
    /// 进程名称
    /// </summary>
    public string ProcessName { get; set; }

    /// <summary>
    /// 内存使用情况。单位：M
    /// </summary>
    public string MemoryUsage { get; set; }

    /// <summary>
    /// CPU使用情况
    /// </summary>
    public string CpuUsage { get; set; }

    /// <summary>
    /// 快照时间
    /// </summary>
    public DateTime Time { get; set; }
}