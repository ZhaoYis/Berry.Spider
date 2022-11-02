using Berry.Spider.Core;
using Volo.Abp.EventBus;

namespace Berry.Spider.TouTiao;

[EventName(RoutingKeyString)]
public class TouTiaoSpider4InformationCompositionPullEto : SpiderPullBaseEto
{
    public const string RoutingKeyString = "Berry.TouTiao.InformationComposition.Pull";
    public const string QueueNameString = "TouTiao.InformationComposition.Pull";

    public TouTiaoSpider4InformationCompositionPullEto() : base(SpiderSourceFrom.TouTiao_Information_Composition)
    {
    }
}