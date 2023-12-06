using Berry.Spider.Core;

namespace Berry.Spider.TouTiao;

[SpiderEventName(EtoType.Push, RoutingKeyString, SpiderSourceFrom.TouTiao_HighQuality_Question_Ext_NO_1)]
public class TouTiaoSpider4HighQualityQuestionExtNo1PushEto : SpiderPushBaseEto
{
    public const string RoutingKeyString = "TouTiao.HighQualityQuestionExtNo1.Push";
    public const string QueueNameString = "Berry.TouTiao.HighQualityQuestionExtNo1.Push";

    public TouTiaoSpider4HighQualityQuestionExtNo1PushEto()
    {
    }

    public TouTiaoSpider4HighQualityQuestionExtNo1PushEto(SpiderSourceFrom from, string keyword, string? traceCode,
        string identityId) : this()
    {
        this.SourceFrom = from;
        this.Keyword = keyword;
        this.TraceCode = traceCode;
        this.IdentityId = identityId;
    }
}