using Berry.Spider.Core;

namespace Berry.Spider.TouTiao;

/// <summary>
/// 头条：问答
/// </summary>
[SpiderEventName(RoutingKeyString, SpiderSourceFrom.TouTiao_Question_Ext_NO_1)]
public class TouTiaoSpider4QuestionExtNo1PullEto : SpiderPullBaseEto
{
    public const string RoutingKeyString = "Berry.TouTiao.QuestionExtNo1.Pull";
    public const string QueueNameString = "TouTiao.QuestionExtNo1.Pull";

    public TouTiaoSpider4QuestionExtNo1PullEto() : base(SpiderSourceFrom.TouTiao_Question_Ext_NO_1)
    {
    }
}