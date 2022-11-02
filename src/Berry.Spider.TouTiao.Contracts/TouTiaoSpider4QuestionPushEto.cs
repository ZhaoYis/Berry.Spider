using Volo.Abp.EventBus;

namespace Berry.Spider.TouTiao;

[EventName(RoutingKeyString)]
public class TouTiaoSpider4QuestionPushEto : SpiderPushBaseEto
{
    public const string RoutingKeyString = "Berry.TouTiao.Question.Push";
    public const string QueueNameString = "TouTiao.Question.Push";
}