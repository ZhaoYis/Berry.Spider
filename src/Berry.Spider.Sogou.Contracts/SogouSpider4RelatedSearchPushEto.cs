using Berry.Spider.Core;

namespace Berry.Spider.Sogou;

[SpiderEventName(EtoType.Push, RoutingKeyString, SpiderSourceFrom.Sogou_Related_Search)]
public class SogouSpider4RelatedSearchPushEto : SpiderPushBaseEto
{
    public const string RoutingKeyString = "Sogou.RelatedSearch.Push";
    public const string QueueNameString = "Berry.Sogou.RelatedSearch.Push";

    public SogouSpider4RelatedSearchPushEto()
    {
    }

    public SogouSpider4RelatedSearchPushEto(SpiderSourceFrom from, string keyword, string? traceCode, string identityId)
        : this()
    {
        this.SourceFrom = from;
        this.Keyword = keyword;
        this.TraceCode = traceCode;
        this.IdentityId = identityId;
    }
}