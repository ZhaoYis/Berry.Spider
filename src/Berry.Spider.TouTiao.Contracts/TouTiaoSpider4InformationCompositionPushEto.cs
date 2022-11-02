using Volo.Abp.EventBus;

namespace Berry.Spider.TouTiao;

[EventName(RoutingKeyString)]
public class TouTiaoSpider4InformationCompositionPushEto : SpiderPushBaseEto
{
    public const string RoutingKeyString = "Berry.TouTiao.InformationComposition.Push";
    public const string QueueNameString = "TouTiao.InformationComposition.Push";
}