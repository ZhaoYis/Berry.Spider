using Berry.Spider.Core;

namespace Berry.Spider.TouTiao;

[SpiderEventName(EtoType.Pull, RoutingKeyString, SpiderSourceFrom.TouTiao_HighQuality_Question)]
public class TouTiaoSpider4HighQualityQuestionPullEto : SpiderPullBaseEto
{
    public const string RoutingKeyString = "Berry.TouTiao.HighQualityQuestion.Pull";
    public const string QueueNameString = "TouTiao.HighQualityQuestion.Pull";

    public TouTiaoSpider4HighQualityQuestionPullEto() : base(SpiderSourceFrom.TouTiao_HighQuality_Question)
    {
    }
    
    public TouTiaoSpider4HighQualityQuestionPullEto(SpiderSourceFrom from, string keyword, string title, List<ChildPageDataItem> items, string? traceCode) : this()
    {
        this.SourceFrom = from;
        this.Keyword = keyword;
        this.Title = title;
        this.Items = items;
        this.TraceCode = traceCode;
    }
}