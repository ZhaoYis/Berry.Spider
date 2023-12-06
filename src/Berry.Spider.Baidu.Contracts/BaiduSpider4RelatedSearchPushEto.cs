using Berry.Spider.Core;

namespace Berry.Spider.Baidu;

[SpiderEventName(EtoType.Push, RoutingKeyString, SpiderSourceFrom.Baidu_Related_Search)]
public class BaiduSpider4RelatedSearchPushEto : SpiderPushBaseEto
{
    public const string RoutingKeyString = "Baidu.RelatedSearch.Push";
    public const string QueueNameString = "Berry.Baidu.RelatedSearch.Push";

    public BaiduSpider4RelatedSearchPushEto()
    {
    }

    public BaiduSpider4RelatedSearchPushEto(SpiderSourceFrom from, string keyword, string? traceCode, string identityId)
        : this()
    {
        this.SourceFrom = from;
        this.Keyword = keyword;
        this.TraceCode = traceCode;
        this.IdentityId = identityId;
    }
}