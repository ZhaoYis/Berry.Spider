using Berry.Spider.Core;

namespace Berry.Spider.TouTiao;

[SpiderEventName(EtoType.Pull, RoutingKeyString, SpiderSourceFrom.TouTiao_Information_Composition)]
public class TouTiaoSpider4InformationCompositionPullEto : SpiderPullBaseEto
{
    public const string RoutingKeyString = "Berry.TouTiao.InformationComposition.Pull";
    public const string QueueNameString = "TouTiao.InformationComposition.Pull";

    public TouTiaoSpider4InformationCompositionPullEto() : base(SpiderSourceFrom.TouTiao_Information_Composition)
    {
    }
    
    public TouTiaoSpider4InformationCompositionPullEto(SpiderSourceFrom from, string keyword, string title, List<ChildPageDataItem> items, string? traceCode) : this()
    {
        this.SourceFrom = from;
        this.Keyword = keyword;
        this.Title = title;
        this.Items = items;
        this.TraceCode = traceCode;
    }
}