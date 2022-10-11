namespace Berry.Spider.FreeRedis;

public class RedisOptions
{
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// 连接字符串
    /// </summary>
    public string Configuration { get; set; }
}