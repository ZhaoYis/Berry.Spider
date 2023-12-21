using Volo.Abp.Domain.Entities.Auditing;

namespace Berry.Spider.Domain;

/// <summary>
/// 爬虫应用信息
/// </summary>
public class SpiderAppInfo : FullAuditedEntity<int>
{
    /// <summary>
    /// 自定义业务ID
    /// </summary>
    public string BizNo { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// tag名称
    /// </summary>
    public string TagName { get; set; }

    /// <summary>
    /// 版本号
    /// </summary>
    public string Version { get; set; }

    /// <summary>
    /// 当前提交记录标识
    /// </summary>
    public string TargetCommitish { get; set; }

    /// <summary>
    /// 包创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 包发布时间
    /// </summary>
    public DateTime PublishedAt { get; set; }

    /// <summary>
    /// OSS存储的文件key
    /// </summary>
    public string OssKey { get; set; }
}