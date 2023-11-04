using Berry.Spider.Core;

namespace Berry.Spider.TouTiao;

[SpiderEventName(EtoType.Push, RoutingKeyString, SpiderSourceFrom.TouTiao_HighQuality_Question)]
public class TouTiaoSpider4HighQualityQuestionPushEto : SpiderPushBaseEto
{
    public const string RoutingKeyString = "Berry.TouTiao.HighQualityQuestion.Push";
    public const string QueueNameString = "TouTiao.HighQualityQuestion.Push";

    public TouTiaoSpider4HighQualityQuestionPushEto()
    {
    }

    public TouTiaoSpider4HighQualityQuestionPushEto(SpiderSourceFrom from, string keyword, string? traceCode,
        string identityId) : this()
    {
        this.SourceFrom = from;
        this.Keyword = keyword;
        this.TraceCode = traceCode;
        this.IdentityId = identityId;
    }
}