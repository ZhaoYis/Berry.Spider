using Berry.Spider.Core;

namespace Berry.Spider.TouTiao;

[SpiderEventName(EtoType.Push, RoutingKeyString, SpiderSourceFrom.TouTiao_Information)]
public class TouTiaoSpider4InformationPushEto : SpiderPushBaseEto
{
    public const string RoutingKeyString = "Berry.TouTiao.Information.Push";
    public const string QueueNameString = "TouTiao.Information.Push";

    public TouTiaoSpider4InformationPushEto()
    {
    }

    public TouTiaoSpider4InformationPushEto(SpiderSourceFrom from, string keyword, string? traceCode) : this()
    {
        this.SourceFrom = from;
        this.Keyword = keyword;
        this.TraceCode = traceCode;
    }
}