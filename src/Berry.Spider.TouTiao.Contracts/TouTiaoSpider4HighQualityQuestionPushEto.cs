using Berry.Spider.Core;

namespace Berry.Spider.TouTiao;

[SpiderEventName(RoutingKeyString, SpiderSourceFrom.TouTiao_HighQuality_Question)]
public class TouTiaoSpider4HighQualityQuestionPushEto : SpiderPushBaseEto
{
    public const string RoutingKeyString = "Berry.TouTiao.HighQualityQuestion.Push";
    public const string QueueNameString = "TouTiao.HighQualityQuestion.Push";
}