﻿using Berry.Spider.Core;

namespace Berry.Spider.TouTiao;

[SpiderEventName(EtoType.Push, RoutingKeyString, SpiderSourceFrom.TouTiao_Question)]
public class TouTiaoSpider4QuestionPushEto : SpiderPushBaseEto
{
    public const string RoutingKeyString = "TouTiao.Question.Push";
    public const string QueueNameString = "Berry.TouTiao.Question.Push";

    public TouTiaoSpider4QuestionPushEto()
    {
    }

    public TouTiaoSpider4QuestionPushEto(SpiderSourceFrom from, string keyword, string? traceCode, string identityId) :
        this()
    {
        this.SourceFrom = from;
        this.Keyword = keyword;
        this.TraceCode = traceCode;
        this.IdentityId = identityId;
    }
}