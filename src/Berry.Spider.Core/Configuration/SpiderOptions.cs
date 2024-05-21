namespace Berry.Spider.Core;

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
    /// 内容需要分段时，每一段的记录数量。默认30条。
    /// </summary>
    public int EverySectionRecords { get; set; } = 30;

    /// <summary>
    /// 内容是否插入图片
    /// </summary>
    public bool IsInsertImage { get; set; }

    /// <summary>
    /// 落库时是否随机插入图片
    /// </summary>
    public bool IsRandomInsertImage { get; set; }

    /// <summary>
    /// 关键字采集是否启用唯一性验证（Push模式）
    /// </summary>
    public bool IsEnablePushUniqVerif { get; set; }

    /// <summary>
    /// 关键字采集是否启用唯一性验证（Pull模式）
    /// </summary>
    public bool IsEnablePullUniqVerif { get; set; }

    /// <summary>
    /// 是否使用naipan伪原创生成内容
    /// </summary>
    public bool IsEnableNaiPan { get; set; }

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

    /// <summary>
    /// 分段标题选项
    /// </summary>
    public SectionTitleOptions SectionTitleOptions { get; set; } = new();

    /// <summary>
    /// 监听服务配置选项
    /// </summary>
    public ServLifetimeOptions ServLifetimeOptions { get; set; } = new();
}