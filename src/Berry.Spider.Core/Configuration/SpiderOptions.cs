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

    /// <summary>
    /// 关键字校验选项
    /// </summary>
    public KeywordCheckOptions KeywordCheckOptions { get; set; } = new();

    /// <summary>
    /// 优质问答选择
    /// </summary>
    public HighQualityAnswerOptions HighQualityAnswerOptions { get; set; } = new();

    /// <summary>
    /// 内容主标题选项
    /// </summary>
    public MainTitleOptions MainTitleOptions { get; set; } = new();
    
    /// <summary>
    /// 内容子标题选项
    /// </summary>
    public SubTitleOptions SubTitleOptions { get; set; } = new();
}