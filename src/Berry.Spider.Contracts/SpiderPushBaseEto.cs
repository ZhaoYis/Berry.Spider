﻿using Berry.Spider.Domain.Shared;

namespace Berry.Spider;

public class SpiderPushBaseEto : ISpiderPushEto
{
    /// <summary>
    /// 来源
    /// </summary>
    public SpiderSourceFrom SourceFrom { get; set; }

    /// <summary>
    /// 关键字
    /// </summary>
    public string Keyword { get; set; }
    // public List<string> Keywords { get; set; } = new List<string>();
}