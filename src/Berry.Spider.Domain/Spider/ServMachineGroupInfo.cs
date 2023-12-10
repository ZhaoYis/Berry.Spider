using Volo.Abp.Domain.Entities.Auditing;

namespace Berry.Spider.Domain;

/// <summary>
/// 机器分组信息
/// </summary>
public class ServMachineGroupInfo : FullAuditedEntity<int>
{
    protected ServMachineGroupInfo()
    {
    }

    /// <summary>
    /// 自定义业务ID
    /// </summary>
    public string BizNo { get; set; }

    /// <summary>
    /// 编码
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string Remark { get; set; }
}