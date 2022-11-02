using Volo.Abp.EventBus;

namespace Berry.Spider.TouTiao;

[EventName(EventNameString)]
public class TouTiaoSpider4InformationPushEto : SpiderPushBaseEto
{
    public const string EventNameString = "Berry.TouTiao.Information.Push";
}