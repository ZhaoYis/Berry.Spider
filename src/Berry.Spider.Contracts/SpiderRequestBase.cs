namespace Berry.Spider.Contracts;

/// <summary>
/// 爬虫请求参数基类
/// </summary>
public class SpiderRequestBase : ISpiderRequest
{
    /// <summary>
    /// 检索关键字
    /// </summary>
    public virtual string Keyword { get; set; }
}