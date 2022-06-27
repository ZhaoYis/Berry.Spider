using System.ComponentModel;

namespace Berry.Spider.Domain.Shared;

/// <summary>
/// 爬虫数据来源
/// </summary>
public enum SpiderSourceFrom
{
    /// <summary>
    /// TXT文件导入
    /// </summary>
    [Description("TXT文件导入")] TextFile_Import = 10,

    /// <summary>
    /// 头条_资讯
    /// </summary>
    [Description("头条_资讯")] TouTiao_Information = 101,

    /// <summary>
    /// 头条_问答
    /// </summary>
    [Description("头条_问答")] TouTiao_Question = 102,

    /// <summary> 
    /// 百度_相关推荐
    /// </summary>
    [Description("百度_相关搜索")] Baidu_RelatedSearch = 201
}