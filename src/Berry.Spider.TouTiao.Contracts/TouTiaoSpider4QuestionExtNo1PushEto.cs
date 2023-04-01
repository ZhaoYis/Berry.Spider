using Berry.Spider.Core;

namespace Berry.Spider.TouTiao;

[SpiderEventName(EtoType.Push, RoutingKeyString, SpiderSourceFrom.TouTiao_Question_Ext_NO_1)]
public class TouTiaoSpider4QuestionExtNo1PushEto : SpiderPushBaseEto
{
    public const string RoutingKeyString = "Berry.TouTiao.QuestionExtNo1.Push";
    public const string QueueNameString = "TouTiao.QuestionExtNo1.Push";

    public TouTiaoSpider4QuestionExtNo1PushEto()
    {
    }

    public TouTiaoSpider4QuestionExtNo1PushEto(SpiderSourceFrom from, string keyword) : this()
    {
        this.SourceFrom = from;
        this.Keyword = keyword;
    }
}