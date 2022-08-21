using Berry.Spider.Core;

namespace Berry.Spider;

/// <summary>
/// 爬虫请求参数基类
/// </summary>
public class SpiderRequestBase : ISpiderRequest
{
    /// <summary>
    /// 检索关键字
    /// </summary>
    public virtual string Keyword { get; set; }

    /// <summary>
    /// 来源
    /// </summary>
    public SpiderSourceFrom SourceFrom { get; set; }
}