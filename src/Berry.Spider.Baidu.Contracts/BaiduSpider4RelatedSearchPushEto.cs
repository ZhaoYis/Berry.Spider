using Berry.Spider.Core;

namespace Berry.Spider.Baidu;

[SpiderEventName(RoutingKeyString, SpiderSourceFrom.Baidu_Related_Search)]
public class BaiduSpider4RelatedSearchPushEto : SpiderPushBaseEto
{
    public const string RoutingKeyString = "Berry.Baidu.RelatedSearch.Push";
    public const string QueueNameString = "Baidu.RelatedSearch.Push";
}