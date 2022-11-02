using Volo.Abp.EventBus;

namespace Berry.Spider.Baidu;

[EventName(RoutingKeyString)]
public class BaiduSpider4RelatedSearchPushEto : SpiderPushBaseEto
{
    public const string RoutingKeyString = "Berry.Baidu.RelatedSearch.Push";
    public const string QueueNameString = "Baidu.RelatedSearch.Push";
}