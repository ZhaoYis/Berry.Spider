namespace Berry.Spider.Contracts;

/// <summary>
/// 爬虫配置选项
/// </summary>
public class SpiderOptions
{
    /// <summary>
    /// 落库每条记录最小内容数量
    /// </summary>
    public int MinRecords { get; set; }
}