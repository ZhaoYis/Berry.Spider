using Volo.Abp.EventBus;

namespace Berry.Spider.TouTiao;

[EventName(RoutingKeyString)]
public class TouTiaoSpider4InformationPushEto : SpiderPushBaseEto
{
    public const string RoutingKeyString = "Berry.TouTiao.Information.Push";
    public const string QueueNameString = "TouTiao.Information.Push";
}