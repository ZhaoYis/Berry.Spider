using System.ComponentModel;

namespace Berry.Spider.Domain.Shared;

/// <summary>
/// 爬虫数据来源
/// </summary>
public enum SpiderSourceFrom
{
    /// <summary>
    /// 头条_资讯
    /// </summary>
    [Description("头条_资讯")]
    TouTiao_Information = 101,
    /// <summary>
    /// 头条_问答
    /// </summary>
    [Description("头条_问答")]
    TouTiao_Question = 102,
}