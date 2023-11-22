using Berry.Spider.Core;

namespace Berry.Spider.TouTiao;

/// <summary>
/// 头条：资讯
/// </summary>
[SpiderEventName(EtoType.Pull, RoutingKeyString, SpiderSourceFrom.TouTiao_Information)]
public class TouTiaoSpider4InformationPullEto : SpiderPullBaseEto
{
    public const string RoutingKeyString = "Berry.TouTiao.Information.Pull";
    public const string QueueNameString = "TouTiao.Information.Pull";

    public TouTiaoSpider4InformationPullEto() : base(SpiderSourceFrom.TouTiao_Information)
    {
    }

    public TouTiaoSpider4InformationPullEto(SpiderSourceFrom from, string keyword, string title,
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