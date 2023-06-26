using Berry.Spider.Core;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Berry.Spider.Domain.Shared;

namespace Berry.Spider.Domain;

/// <summary>
/// 根据关键字抓取的一级页面标题信息
/// </summary>
public class SpiderContent_Keyword : EntityBase, ITraceCode
{
    public SpiderContent_Keyword()
    {
    }

    public SpiderContent_Keyword(
        string title,
        SpiderSourceFrom from,
        string? traceCode)
    {
        this.Title = title;
        this.From = from.ToString();
        this.Time = DateTimeOffset.Now.DateTime;
        this.TraceCode = traceCode;
    }

    /// <summary>
    /// 标题
    /// </summary>
    [Column("标题"), Required]
    public string Title { get; set; }

    /// <summary>
    /// 时间
    /// </summary>
    [Column("时间")]
    public DateTime Time { get; set; }

    /// <summary>
    /// 出处
    /// </summary>
    [Column("出处"), AllowNull]
    public string From { get; set; }

    /// <summary>
    /// 用作当前关键字最初导入时的批次追踪标识
    /// </summary>
    public string? TraceCode { get; set; }
}