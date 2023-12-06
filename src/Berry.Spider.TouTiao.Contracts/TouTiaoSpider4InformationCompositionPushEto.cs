using Berry.Spider.Core;

namespace Berry.Spider.TouTiao;

[SpiderEventName(EtoType.Push, RoutingKeyString, SpiderSourceFrom.TouTiao_Information_Composition)]
public class TouTiaoSpider4InformationCompositionPushEto : SpiderPushBaseEto
{
    public const string RoutingKeyString = "TouTiao.InformationComposition.Push";
    public const string QueueNameString = "Berry.TouTiao.InformationComposition.Push";

    public TouTiaoSpider4InformationCompositionPushEto()
    {
    }

    public TouTiaoSpider4InformationCompositionPushEto(SpiderSourceFrom from, string keyword, string? traceCode,
        string identityId) : this()
    {
        this.SourceFrom = from;
        this.Keyword = keyword;
        this.TraceCode = traceCode;
        this.IdentityId = identityId;
    }
}