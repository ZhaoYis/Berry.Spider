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

    /// <summary>
    /// 落库每条记录每行最小字符数
    /// </summary>
    public int MinContentLength { get; set; }

    /// <summary>
    /// 内容是否插入图片
    /// </summary>
    public bool IsInsertImage { get; set; }

    /// <summary>
    /// 落库时是否随机插入图片
    /// </summary>
    public bool IsRandomInsertImage { get; set; }
}