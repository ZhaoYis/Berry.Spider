using Berry.Spider.Core;
using Volo.Abp.EventBus;

namespace Berry.Spider.TouTiao;

[EventName("Berry.TouTiao.InformationComposition.Pull")]
public class TouTiaoSpider4InformationCompositionPullEto : SpiderPullBaseEto
{
    public TouTiaoSpider4InformationCompositionPullEto() : base(SpiderSourceFrom.TouTiao_Information_Composition)
    {
    }
}