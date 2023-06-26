using Berry.Spider.Core;
using JetBrains.Annotations;

namespace Berry.Spider.Domain;

/// <summary>
/// 爬虫抓取下来的内容（优质问答）
/// </summary>
public class SpiderContent_HighQualityQA : SpiderContentBase
{
    protected SpiderContent_HighQualityQA()
    {
    }

    /// <summary>
    /// 当前关键字查询出来的列表归属组ID
    /// </summary>
    public string? GroupId { get; set; }

    public SpiderContent_HighQualityQA([NotNull] string title,
        [NotNull] string content, SpiderSourceFrom @from, [CanBeNull] string? pageUrl = "",
        [CanBeNull] string? keywords = "", [CanBeNull] string? tag = "") : base(title, content, @from, pageUrl,
        keywords, tag)
    {
    }

    public SpiderContent_HighQualityQA([NotNull] string title,
        [NotNull] string content, string groupId, SpiderSourceFrom @from, [CanBeNull] string? pageUrl = "",
        [CanBeNull] string? keywords = "", [CanBeNull] string? tag = "") : base(title, content, @from, pageUrl,
        keywords, tag)
    {
        this.GroupId = groupId;
    }
}