﻿using Berry.Spider.Core;
using Volo.Abp.EventBus;

namespace Berry.Spider.TouTiao;

/// <summary>
/// 头条：问答
/// </summary>
[EventName("Berry.TouTiao.Question.Pull")]
public class TouTiaoSpider4QuestionEto : SpiderPullBaseEto
{
    public TouTiaoSpider4QuestionEto() : base(SpiderSourceFrom.TouTiao_Question)
    {
    }
}