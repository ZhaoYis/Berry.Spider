using Berry.Spider.Core;

namespace Berry.Spider.TouTiao;

[SpiderEventName(RoutingKeyString, SpiderSourceFrom.TouTiao_Information_Composition)]
public class TouTiaoSpider4InformationCompositionPushEto : SpiderPushBaseEto
{
    public const string RoutingKeyString = "Berry.TouTiao.InformationComposition.Push";
    public const string QueueNameString = "TouTiao.InformationComposition.Push";
}