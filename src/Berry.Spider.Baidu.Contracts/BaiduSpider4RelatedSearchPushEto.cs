using Berry.Spider.Core;

namespace Berry.Spider.Baidu;

[SpiderEventName(EtoType.Push, RoutingKeyString, SpiderSourceFrom.Baidu_Related_Search)]
public class BaiduSpider4RelatedSearchPushEto : SpiderPushBaseEto
{
    public const string RoutingKeyString = "Berry.Baidu.RelatedSearch.Push";
    public const string QueueNameString = "Baidu.RelatedSearch.Push";
    
    public BaiduSpider4RelatedSearchPushEto()
    {
    }

    public BaiduSpider4RelatedSearchPushEto(SpiderSourceFrom from, string keyword, string? traceCode) : this()
    {
        this.SourceFrom = from;
        this.Keyword = keyword;
        this.TraceCode = traceCode;
    }
}