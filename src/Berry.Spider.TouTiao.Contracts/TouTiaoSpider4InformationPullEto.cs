using Berry.Spider.Core;
using Volo.Abp.EventBus;

namespace Berry.Spider.TouTiao;

/// <summary>
/// 头条：资讯
/// </summary>
[EventName("Berry.TouTiao.Information.Pull")]
public class TouTiaoSpider4InformationPullEto : SpiderPullBaseEto
{
    public TouTiaoSpider4InformationPullEto() : base(SpiderSourceFrom.TouTiao_Information)
    {
    }
}