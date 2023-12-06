using Berry.Spider.Core;

namespace Berry.Spider.TouTiao;

/// <summary>
/// 头条：问答
/// </summary>
[SpiderEventName(EtoType.Pull, RoutingKeyString, SpiderSourceFrom.TouTiao_Question)]
public class TouTiaoSpider4QuestionPullEto : SpiderPullBaseEto
{
    public const string RoutingKeyString = "TouTiao.Question.Pull";
    public const string QueueNameString = "Berry.TouTiao.Question.Pull";

    public TouTiaoSpider4QuestionPullEto() : base(SpiderSourceFrom.TouTiao_Question)
    {
    }

    public TouTiaoSpider4QuestionPullEto(SpiderSourceFrom from, string keyword, string title,
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