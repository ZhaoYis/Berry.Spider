using Volo.Abp.EventBus;

namespace Berry.Spider.Sogou;

[EventName(EventNameString)]
public class SogouSpider4RelatedSearchPushEto : SpiderPushBaseEto
{
    public const string EventNameString = "Berry.Sogou.RelatedSearch.Push";
}