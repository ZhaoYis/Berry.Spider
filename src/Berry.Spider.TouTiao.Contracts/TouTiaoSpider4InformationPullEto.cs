using Berry.Spider.Core;
using Volo.Abp.EventBus;

namespace Berry.Spider.TouTiao;

/// <summary>
/// 头条：资讯
/// </summary>
[EventName(RoutingKeyString)]
public class TouTiaoSpider4InformationPullEto : SpiderPullBaseEto
{
    public const string RoutingKeyString = "Berry.TouTiao.Information.Pull";
    public const string QueueNameString = "TouTiao.Information.Pull";

    public TouTiaoSpider4InformationPullEto() : base(SpiderSourceFrom.TouTiao_Information)
    {
    }
}