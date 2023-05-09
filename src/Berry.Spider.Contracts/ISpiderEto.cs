using Berry.Spider.Core;

namespace Berry.Spider;

public interface ISpiderEto
{
    /// <summary>
    /// 爬虫数据来源
    /// </summary>
    SpiderSourceFrom SourceFrom { get; set; }
}