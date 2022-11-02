using Volo.Abp.EventBus;

namespace Berry.Spider.Baidu;

[EventName(EventNameString)]
public class BaiduSpider4RelatedSearchPushEto : SpiderPushBaseEto
{
    public const string EventNameString = "Berry.Baidu.RelatedSearch.Push";
}