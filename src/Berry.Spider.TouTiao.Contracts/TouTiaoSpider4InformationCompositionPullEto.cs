using Berry.Spider.Core;

namespace Berry.Spider.TouTiao;

[SpiderEventName(EtoType.Pull, RoutingKeyString, SpiderSourceFrom.TouTiao_Information_Composition)]
public class TouTiaoSpider4InformationCompositionPullEto : SpiderPullBaseEto
{
    public const string RoutingKeyString = "TouTiao.InformationComposition.Pull";
    public const string QueueNameString = "Berry.TouTiao.InformationComposition.Pull";

    public TouTiaoSpider4InformationCompositionPullEto() : base(SpiderSourceFrom.TouTiao_Information_Composition)
    {
    }

    public TouTiaoSpider4InformationCompositionPullEto(SpiderSourceFrom from, string keyword, string title,
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