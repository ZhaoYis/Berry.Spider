namespace Berry.Spider.Core;

public class MongoDBOptions
{
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// 数据库连接字符串
    /// </summary>
    public string ConnectionString { get; set; }

    /// <summary>
    /// 是否启用分布式锁
    /// </summary>
    public bool UseStorageLock { get; set; } = false;
}