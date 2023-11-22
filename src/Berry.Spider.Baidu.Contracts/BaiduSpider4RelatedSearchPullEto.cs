using Berry.Spider.Core;

namespace Berry.Spider.Baidu;

/// <summary>
/// 百度：相关搜索
/// </summary>
[SpiderEventName(EtoType.Pull, RoutingKeyString, SpiderSourceFrom.Baidu_Related_Search)]
public class BaiduSpider4RelatedSearchPullEto : SpiderPullBaseEto
{
    public const string RoutingKeyString = "Berry.Baidu.RelatedSearch.Pull";
    public const string QueueNameString = "Baidu.RelatedSearch.Pull";

    public BaiduSpider4RelatedSearchPullEto() : base(SpiderSourceFrom.Baidu_Related_Search)
    {
    }

    public BaiduSpider4RelatedSearchPullEto(SpiderSourceFrom from, string keyword, string title,
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