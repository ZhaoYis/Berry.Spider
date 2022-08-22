using Berry.Spider.Core;
using Volo.Abp.EventBus;

namespace Berry.Spider.TouTiao;

/// <summary>
/// 头条：问答
/// </summary>
[EventName("Berry.TouTiao.Question.Pull")]
public class TouTiaoSpider4QuestionPullEto : SpiderPullBaseEto
{
    public TouTiaoSpider4QuestionPullEto() : base(SpiderSourceFrom.TouTiao_Question)
    {
    }
}