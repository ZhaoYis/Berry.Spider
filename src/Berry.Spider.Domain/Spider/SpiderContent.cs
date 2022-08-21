using Berry.Spider.Core;
using JetBrains.Annotations;

namespace Berry.Spider.Domain;

/// <summary>
/// 爬虫抓取下来的内容
/// </summary>
public class SpiderContent : SpiderContentBase
{
    protected SpiderContent()
    {
    }

    public SpiderContent([NotNull] string title,
        [NotNull] string content, SpiderSourceFrom @from, [CanBeNull] string? pageUrl = "",
        [CanBeNull] string? keywords = "", [CanBeNull] string? tag = "") : base(title, content, @from, pageUrl,
        keywords, tag)
    {
    }
}