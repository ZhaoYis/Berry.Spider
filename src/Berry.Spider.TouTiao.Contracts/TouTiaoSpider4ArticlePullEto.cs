using Berry.Spider.Core;

namespace Berry.Spider.TouTiao;

/// <summary>
/// 头条：文章
/// </summary>
[SpiderEventName(EtoType.Pull, RoutingKeyString, SpiderSourceFrom.TouTiao_Information)]
public class TouTiaoSpider4ArticlePullEto : SpiderPullBaseEto
{
    public const string RoutingKeyString = "TouTiao.Article.Pull";
    public const string QueueNameString = "Berry.TouTiao.Article.Pull";

    public TouTiaoSpider4ArticlePullEto() : base(SpiderSourceFrom.TouTiao_Information)
    {
    }

    public TouTiaoSpider4ArticlePullEto(SpiderSourceFrom from, string keyword, string title,
        List<ChildPageDataItem> items, string? traceCode, string identityId) : this()
    {
        this.SourceFrom = from;
        this.Keyword = keyword;
        this.Title = title;
        this.Items = items;
        this.TraceCode = traceCode;
        this.IdentityId = identityId;
    }
}