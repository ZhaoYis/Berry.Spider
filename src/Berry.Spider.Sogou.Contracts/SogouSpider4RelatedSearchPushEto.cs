using Berry.Spider.Core;

namespace Berry.Spider.Sogou;

[SpiderEventName(EtoType.Push, RoutingKeyString, SpiderSourceFrom.Sogou_Related_Search)]
public class SogouSpider4RelatedSearchPushEto : SpiderPushBaseEto
{
    public const string RoutingKeyString = "Berry.Sogou.RelatedSearch.Push";
    public const string QueueNameString = "Sogou.RelatedSearch.Push";

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