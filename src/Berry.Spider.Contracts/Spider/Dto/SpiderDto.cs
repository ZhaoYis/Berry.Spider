using Berry.Spider.Core;
using Volo.Abp.Application.Dtos;

namespace Berry.Spider;

/// <summary>
/// 爬虫信息
/// </summary>
public class SpiderDto : EntityDto<int>
{
    /// <summary>
    /// 爬虫名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 爬虫来源类型
    /// </summary>
    public SpiderSourceFrom From { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string Remark { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreationTime { get; }

    /// <summary>
    /// 最后一次更新时间
    /// </summary>
    public DateTime? LastModificationTime { get; set; }
}