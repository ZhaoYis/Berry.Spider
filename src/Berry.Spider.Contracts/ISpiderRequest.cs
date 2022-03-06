using Berry.Spider.Domain.Shared;

namespace Berry.Spider.Contracts;

/// <summary>
/// 爬虫公共请求实体
/// </summary>
public interface ISpiderRequest
{
    /// <summary>
    /// 检索关键字
    /// </summary>
    string Keyword { get; set; }

    /// <summary>
    /// 来源
    /// </summary>
    SpiderSourceFrom SourceFrom { get; set; }
}