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
    /// JSON文件导入
    /// </summary>
    [Description("JSON文件导入")] Json_File_Import = 30,

    /// <summary>
    /// 头条_资讯
    /// </summary>
    [Description("头条_资讯")] TouTiao_Information = 101,

    /// <summary>
    /// 头条_资讯_作文板块
    /// </summary>
    [Description("头条_资讯_作文板块")] TouTiao_Information_Composition = 102,

    #region 头条_问答

    /// <summary>
    /// 头条_问答
    /// </summary>
    [Description("头条_问答")] TouTiao_Question = 110,

    /// <summary>
    /// 头条_问答_扩展_01
    /// </summary>
    [Description("头条_问答_扩展_01")] TouTiao_Question_Ext_NO_1 = 2110,

    #endregion

    #region 头条_优质_问答

    /// <summary>
    /// 头条_优质_问答
    /// </summary>
    [Description("头条_优质_问答")] TouTiao_HighQuality_Question = 111,

    /// <summary>
    /// 头条_优质_问答_扩展_01
    /// </summary>
    [Description("头条_优质_问答_扩展_01")] TouTiao_HighQuality_Question_Ext_NO_1 = 2111,

    #endregion

    /// <summary> 
    /// 百度_相关推荐
    /// </summary>
    [Description("百度_相关搜索")] Baidu_Related_Search = 201,

    /// <summary> 
    /// 搜狗_相关搜索
    /// </summary>
    [Description("搜狗_相关搜索")] Sogou_Related_Search = 301,

    /// <summary> 
    /// 搜狗_问问
    /// </summary>
    [Description("搜狗_问问")] Sogou_WenWen = 401,

    /// <summary> 
    /// AI
    /// </summary>
    [Description("AI")] AI = 501
}