using Berry.Spider.Core;

namespace Berry.Spider.TouTiao;

[SpiderEventName(RoutingKeyString, SpiderSourceFrom.TouTiao_Question_Ext_NO_1)]
public class TouTiaoSpider4QuestionExtNo1PushEto : SpiderPushBaseEto
{
    public const string RoutingKeyString = "Berry.TouTiao.QuestionExtNo1.Push";
    public const string QueueNameString = "TouTiao.QuestionExtNo1.Push";
}