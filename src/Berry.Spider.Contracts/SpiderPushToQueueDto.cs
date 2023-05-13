using Berry.Spider.Core;
using Berry.Spider.Domain.Shared;

namespace Berry.Spider;

public class SpiderPushToQueueDto : ITraceCode
{
    public SpiderPushToQueueDto(string keyword, SpiderSourceFrom from, string? traceCode)
    {
        this.SourceFrom = from;
        this.Keyword = keyword;
        this.TraceCode = traceCode;
    }

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
    public string Keyword { get; set; }
}