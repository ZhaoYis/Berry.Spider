using Volo.Abp.EventBus;

namespace Berry.Spider.TouTiao;

[EventName(EventNameString)]
public class TouTiaoSpider4InformationCompositionPushEto : SpiderPushBaseEto
{
    public const string EventNameString = "Berry.TouTiao.InformationComposition.Push";
}