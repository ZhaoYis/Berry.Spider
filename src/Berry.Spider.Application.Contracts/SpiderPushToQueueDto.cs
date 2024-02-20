using System.Text;
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

    /// <summary>
    /// 计算当前入队组合唯一标识
    /// </summary>
    /// <returns></returns>
    public string GetIdentityId()
    {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(this.ToString()));
    }

    /// <summary>Returns a string that represents the current object.</summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        return $"SourceFrom={this.SourceFrom.ToString()}&TraceCode={this.TraceCode}&Keyword={this.Keyword}";
    }
}