using Berry.Spider.Core;

namespace Berry.Spider.Sogou;

[SpiderEventName(RoutingKeyString, SpiderSourceFrom.Sogou_Related_Search)]
public class SogouSpider4RelatedSearchPushEto : SpiderPushBaseEto
{
    public const string RoutingKeyString = "Berry.Sogou.RelatedSearch.Push";
    public const string QueueNameString = "Sogou.RelatedSearch.Push";
}