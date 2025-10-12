using Berry.Spider.Core;

namespace Berry.Spider.TouTiao;

/// <summary>
/// 头条：头条_微头条
/// </summary>
[SpiderEventName(EtoType.Push, RoutingKeyString, SpiderSourceFrom.TouTiao_WeiTouTiao)]
public class TouTiaoSpider4ArticlePushEto : SpiderPushBaseEto
{
    public const string RoutingKeyString = "TouTiao.WeiTouTiao.Push";
    public const string QueueNameString = "Berry.TouTiao.WeiTouTiao.Push";

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