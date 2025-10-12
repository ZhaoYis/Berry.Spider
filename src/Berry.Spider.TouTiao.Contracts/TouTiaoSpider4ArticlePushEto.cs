using Berry.Spider.Core;

namespace Berry.Spider.TouTiao;

[SpiderEventName(EtoType.Push, RoutingKeyString, SpiderSourceFrom.TouTiao_WeiTouTiao)]
public class TouTiaoSpider4ArticlePushEto : SpiderPushBaseEto
{
    public const string RoutingKeyString = "TouTiao.Article.Push";
    public const string QueueNameString = "Berry.TouTiao.Article.Push";

    public TouTiaoSpider4ArticlePushEto()
    {
    }

    public TouTiaoSpider4ArticlePushEto(SpiderSourceFrom from, string keyword, string? traceCode, string identityId)
        : this()
    {
        this.SourceFrom = from;
        this.Keyword = keyword;
        this.TraceCode = traceCode;
        this.IdentityId = identityId;
    }
}