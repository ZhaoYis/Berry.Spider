using Berry.Spider.Core;

namespace Berry.Spider;

public class SpiderPushBaseEto : ISpiderPushEto
{
    /// <summary>
    /// 来源
    /// </summary>
    public SpiderSourceFrom SourceFrom { get; set; }

    /// <summary>
    /// 用作当前关键字最初导入时的批次追踪标识
    /// </summary>
    public string? TraceCode { get; set; }

    /// <summary>
    /// 关键字
    /// </summary>
    public string Keyword { get; set; } = null!;
}