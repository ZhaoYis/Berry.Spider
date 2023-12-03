using Berry.Spider.Core;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Berry.Spider.Domain.Shared;

namespace Berry.Spider.Domain;

public class SpiderContentBase : EntityBase, ITraceCode, ISourceIdentityId
{
    /// <summary>
    /// 已采集
    /// </summary>
    [Column("已采")]
    public int Collected { get; set; } = 1;

    /// <summary>
    /// 已发布
    /// </summary>
    [Column("已发")]
    public int Published { get; set; } = 0;

    /// <summary>
    /// 标题
    /// </summary>
    [Column("标题"), Required]
    public string Title { get; set; }

    /// <summary>
    /// 内容
    /// </summary>
    [Column("内容"), Required]
    public string Content { get; set; }

    /// <summary>
    /// 作者
    /// </summary>
    [Column("作者")]
    public string? Author { get; set; }

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
    /// 页面地址
    /// </summary>
    [Column("PageUrl")]
    public string? PageUrl { get; set; }

    /// <summary>
    /// 关键字
    /// </summary>
    [Column("关键字")]
    public string? Keywords { get; set; }

    /// <summary>
    /// 标签
    /// </summary>
    [Column("tag")]
    public string? Tag { get; set; }

    /// <summary>
    /// 用作当前关键字最初导入时的批次追踪标识
    /// </summary>
    public string? TraceCode { get; set; }

    /// <summary>
    /// 当前采集关键字唯一标识
    /// </summary>
    public string? IdentityId { get; set; }

    protected SpiderContentBase()
    {
    }

    protected SpiderContentBase(
        string title,
        string content,
        SpiderSourceFrom from,
        string? pageUrl = "",
        string? keywords = "",
        string? tag = "")
    {
        this.Title = title;
        this.Content = content;
        this.Author = from.ToString();
        this.From = from.ToString();
        this.Time = DateTimeOffset.Now.DateTime;
        this.PageUrl = pageUrl;
        this.Keywords = keywords ?? "";
        this.Tag = tag ?? "";
    }

    /// <summary>
    /// 设置追踪标识
    /// </summary>
    public void SetTraceCodeIfNotNull(string? traceCode)
    {
        if (!string.IsNullOrEmpty(traceCode))
        {
            this.TraceCode = traceCode;
        }
    }

    /// <summary>
    /// 当前采集关键字唯一标识
    /// </summary>
    public void SetIdentityIdIfNotNull(string? identityId)
    {
        if (!string.IsNullOrEmpty(identityId))
        {
            this.IdentityId = identityId;
        }
    }
}