using Berry.Spider.Core;

namespace Berry.Spider.TouTiao;

[SpiderEventName(RoutingKeyString, SpiderSourceFrom.TouTiao_HighQuality_Question)]
public class TouTiaoSpider4HighQualityQuestionPullEto : SpiderPullBaseEto
{
    public const string RoutingKeyString = "Berry.TouTiao.HighQualityQuestion.Pull";
    public const string QueueNameString = "TouTiao.HighQualityQuestion.Pull";

    public TouTiaoSpider4HighQualityQuestionPullEto() : base(SpiderSourceFrom.TouTiao_HighQuality_Question)
    {
    }
}