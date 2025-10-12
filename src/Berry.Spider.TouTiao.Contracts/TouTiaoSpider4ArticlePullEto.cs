using Berry.Spider.Core;

namespace Berry.Spider.TouTiao;

/// <summary>
/// 头条：头条_微头条
/// </summary>
[SpiderEventName(EtoType.Pull, RoutingKeyString, SpiderSourceFrom.TouTiao_WeiTouTiao)]
public class TouTiaoSpider4ArticlePullEto : SpiderPullBaseEto
{
    public const string RoutingKeyString = "TouTiao.Article.Pull";
    public const string QueueNameString = "Berry.TouTiao.Article.Pull";

    public TouTiaoSpider4ArticlePullEto() : base(SpiderSourceFrom.TouTiao_WeiTouTiao)
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