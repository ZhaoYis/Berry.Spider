using Volo.Abp.EventBus;

namespace Berry.Spider.Sogou;

[EventName(RoutingKeyString)]
public class SogouSpider4RelatedSearchPushEto : SpiderPushBaseEto
{
    public const string RoutingKeyString = "Berry.Sogou.RelatedSearch.Push";
    public const string QueueNameString = "Sogou.RelatedSearch.Push";
}