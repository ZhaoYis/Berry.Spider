namespace Berry.Spider.Domain.Shared;

public interface ITraceCode
{
    /// <summary>
    /// 用作当前关键字最初导入时的批次追踪标识
    /// </summary>
    string? TraceCode { get; set; }
}