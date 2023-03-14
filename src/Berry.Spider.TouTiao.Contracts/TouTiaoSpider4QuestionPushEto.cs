using Berry.Spider.Core;

namespace Berry.Spider.TouTiao;

[SpiderEventName(RoutingKeyString, SpiderSourceFrom.TouTiao_Question)]
public class TouTiaoSpider4QuestionPushEto : SpiderPushBaseEto
{
    public const string RoutingKeyString = "Berry.TouTiao.Question.Push";
    public const string QueueNameString = "TouTiao.Question.Push";
}