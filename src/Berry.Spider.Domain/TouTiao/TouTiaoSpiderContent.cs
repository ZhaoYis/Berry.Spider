using Berry.Spider.Domain.Shared;
using JetBrains.Annotations;

namespace Berry.Spider.Domain.TouTiao;

/// <summary>
/// 今日头条
/// </summary>
public class TouTiaoSpiderContent : SpiderContentBase
{
    protected TouTiaoSpiderContent() : base()
    {
    }

    public TouTiaoSpiderContent([NotNull] string title,
        [NotNull] string content, SpiderSourceFrom @from, [CanBeNull] string? pageUrl = "",
        [CanBeNull] string? keywords = "", [CanBeNull] string? tag = "") : base(title, content, @from, pageUrl,
        keywords, tag)
    {
    }
}