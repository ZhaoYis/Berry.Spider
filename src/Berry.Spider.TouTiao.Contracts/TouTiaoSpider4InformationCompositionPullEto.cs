using Berry.Spider.Core;

namespace Berry.Spider.TouTiao;

[SpiderEventName(RoutingKeyString, SpiderSourceFrom.TouTiao_Information_Composition)]
public class TouTiaoSpider4InformationCompositionPullEto : SpiderPullBaseEto
{
    public const string RoutingKeyString = "Berry.TouTiao.InformationComposition.Pull";
    public const string QueueNameString = "TouTiao.InformationComposition.Pull";

    public TouTiaoSpider4InformationCompositionPullEto() : base(SpiderSourceFrom.TouTiao_Information_Composition)
    {
    }
}