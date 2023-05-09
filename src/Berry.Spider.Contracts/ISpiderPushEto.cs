using Berry.Spider.Domain.Shared;

namespace Berry.Spider;

public interface ISpiderPushEto : ISpiderEto, ITraceCode
{
    /// <summary>
    /// 关键字
    /// </summary>
    string Keyword { get; set; }
}