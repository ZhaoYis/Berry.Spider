using Berry.Spider.Core;

namespace Berry.Spider.TouTiao;

/// <summary>
/// 头条：问答
/// </summary>
[SpiderEventName(EtoType.Pull, RoutingKeyString, SpiderSourceFrom.TouTiao_Question)]
public class TouTiaoSpider4QuestionPullEto : SpiderPullBaseEto
{
    public const string RoutingKeyString = "Berry.TouTiao.Question.Pull";
    public const string QueueNameString = "TouTiao.Question.Pull";

    public TouTiaoSpider4QuestionPullEto() : base(SpiderSourceFrom.TouTiao_Question)
    {
    }

    public TouTiaoSpider4QuestionPullEto(SpiderSourceFrom from, string keyword, string title, List<ChildPageDataItem> items) : this()
    {
        this.SourceFrom = from;
        this.Keyword = keyword;
        this.Title = title;
        this.Items = items;
    }
}