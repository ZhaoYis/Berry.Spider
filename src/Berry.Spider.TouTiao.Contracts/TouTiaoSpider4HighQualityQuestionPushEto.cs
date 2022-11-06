using Volo.Abp.EventBus;

namespace Berry.Spider.TouTiao;

[EventName(RoutingKeyString)]
public class TouTiaoSpider4HighQualityQuestionPushEto : SpiderPushBaseEto
{
    public const string RoutingKeyString = "Berry.TouTiao.HighQualityQuestion.Push";
    public const string QueueNameString = "TouTiao.HighQualityQuestion.Push";
}