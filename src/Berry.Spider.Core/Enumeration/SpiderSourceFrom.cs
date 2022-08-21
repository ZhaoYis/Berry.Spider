using System.ComponentModel;

namespace Berry.Spider.Core;

/// <summary>
/// 爬虫数据来源
/// </summary>
public enum SpiderSourceFrom
{
    /// <summary>
    /// TXT文件导入
    /// </summary>
    [Description("TXT文件导入")] Text_File_Import = 10,

    /// <summary>
    /// EXCEL文件导入
    /// </summary>
    [Description("EXCEL文件导入")] Excel_File_Import = 20,

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
    [Description("百度_相关搜索")] Baidu_Related_Search = 201,

    /// <summary> 
    /// 搜狗_相关搜索
    /// </summary>
    [Description("搜狗_相关搜索")] Sogou_Related_Search = 301
}