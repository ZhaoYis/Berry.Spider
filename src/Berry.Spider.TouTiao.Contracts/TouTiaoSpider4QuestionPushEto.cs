using Volo.Abp.EventBus;

namespace Berry.Spider.TouTiao;

[EventName(EventNameString)]
public class TouTiaoSpider4QuestionPushEto : SpiderPushBaseEto
{
    public const string EventNameString = "Berry.TouTiao.Question.Push";
}