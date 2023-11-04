using Berry.Spider.Core;

namespace Berry.Spider.Sogou;

/// <summary>
/// 搜狗：相关搜索
/// </summary>
[SpiderEventName(EtoType.Pull, RoutingKeyString, SpiderSourceFrom.Sogou_Related_Search)]
public class SogouSpider4RelatedSearchPullEto : SpiderPullBaseEto
{
    public const string RoutingKeyString = "Berry.Sogou.RelatedSearch.Pull";
    public const string QueueNameString = "Sogou.RelatedSearch.Pull";

    public SogouSpider4RelatedSearchPullEto() : base(SpiderSourceFrom.Sogou_Related_Search)
    {
    }

    public SogouSpider4RelatedSearchPullEto(SpiderSourceFrom from, string keyword, string title,
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