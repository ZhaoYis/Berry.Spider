using Berry.Spider.Core;
using Volo.Abp.EventBus;

namespace Berry.Spider.TouTiao;

[EventName(EventNameString)]
public class TouTiaoSpider4InformationCompositionPullEto : SpiderPullBaseEto
{
    public const string EventNameString = "Berry.TouTiao.InformationComposition.Pull";

    public TouTiaoSpider4InformationCompositionPullEto() : base(SpiderSourceFrom.TouTiao_Information_Composition)
    {
    }
}