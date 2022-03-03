using Berry.Spider.Contracts;
using Berry.Spider.Domain.Shared;
using Volo.Abp.EventBus;

namespace Berry.Spider.TouTiao.Contracts;

/// <summary>
/// 头条：资讯
/// </summary>
[EventName("Berry.TouTiao.Information")]
public class TouTiaoSpider4InformationEto : SpiderBaseEto
{
    public TouTiaoSpider4InformationEto(SpiderSourceFrom @from) : base(@from)
    {
    }
}