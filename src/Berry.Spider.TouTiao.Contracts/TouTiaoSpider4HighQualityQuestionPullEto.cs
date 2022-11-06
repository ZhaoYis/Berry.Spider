using Berry.Spider.Core;
using Volo.Abp.EventBus;

namespace Berry.Spider.TouTiao;

[EventName(RoutingKeyString)]
public class TouTiaoSpider4HighQualityQuestionPullEto : SpiderPullBaseEto
{
    public const string RoutingKeyString = "Berry.TouTiao.HighQualityQuestion.Pull";
    public const string QueueNameString = "TouTiao.HighQualityQuestion.Pull";

    public TouTiaoSpider4HighQualityQuestionPullEto() : base(SpiderSourceFrom.TouTiao_HighQuality_Question)
    {
    }
}