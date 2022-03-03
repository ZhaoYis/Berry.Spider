using Berry.Spider.Contracts;
using Berry.Spider.Domain.Shared;
using Volo.Abp.EventBus;

namespace Berry.Spider.TouTiao.Contracts;

/// <summary>
/// 头条：问答
/// </summary>
[EventName("Berry.TouTiao.Question")]
public class TouTiaoSpider4QuestionEto : SpiderBaseEto
{
    public TouTiaoSpider4QuestionEto() : base(SpiderSourceFrom.TouTiao_Question)
    {
    }
}