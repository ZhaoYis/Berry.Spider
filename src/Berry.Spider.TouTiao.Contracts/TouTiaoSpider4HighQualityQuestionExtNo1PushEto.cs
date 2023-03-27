using Berry.Spider.Core;

namespace Berry.Spider.TouTiao;

[SpiderEventName(RoutingKeyString, SpiderSourceFrom.TouTiao_HighQuality_Question_Ext_NO_1)]
public class TouTiaoSpider4HighQualityQuestionExtNo1PushEto : SpiderPushBaseEto
{
    public const string RoutingKeyString = "Berry.TouTiao.HighQualityQuestionExtNo1.Push";
    public const string QueueNameString = "TouTiao.HighQualityQuestionExtNo1.Push";
}