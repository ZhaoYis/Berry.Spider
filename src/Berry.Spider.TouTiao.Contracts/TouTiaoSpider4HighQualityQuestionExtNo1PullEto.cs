using Berry.Spider.Core;

namespace Berry.Spider.TouTiao;

[SpiderEventName(EtoType.Pull, RoutingKeyString, SpiderSourceFrom.TouTiao_HighQuality_Question_Ext_NO_1)]
public class TouTiaoSpider4HighQualityQuestionExtNo1PullEto : SpiderPullBaseEto
{
    public const string RoutingKeyString = "Berry.TouTiao.HighQualityQuestionExtNo1.Pull";
    public const string QueueNameString = "TouTiao.HighQualityQuestionExtNo1.Pull";

    public TouTiaoSpider4HighQualityQuestionExtNo1PullEto() : base(SpiderSourceFrom
        .TouTiao_HighQuality_Question_Ext_NO_1)
    {
    }

    public TouTiaoSpider4HighQualityQuestionExtNo1PullEto(SpiderSourceFrom from, string keyword, string title,
        List<ChildPageDataItem> items, string? traceCode, string identityId) : this()
    {
        this.SourceFrom = from;
        this.Keyword = keyword;
        this.Title = title;
        this.Items = items;
        this.TraceCode = traceCode;
        this.IdentityId = identityId;
    }
}