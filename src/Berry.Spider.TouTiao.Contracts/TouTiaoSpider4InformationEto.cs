using Berry.Spider.Domain.Shared;
using Volo.Abp.EventBus;

namespace Berry.Spider.TouTiao;

/// <summary>
/// 头条：资讯
/// </summary>
[EventName("Berry.TouTiao.Information")]
public class TouTiaoSpider4InformationEto : SpiderPullBaseEto
{
    public TouTiaoSpider4InformationEto() : base(SpiderSourceFrom.TouTiao_Information)
    {
    }
}