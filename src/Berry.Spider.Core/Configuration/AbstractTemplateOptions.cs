using Volo.Abp;

namespace Berry.Spider.Contracts;

/// <summary>
/// 摘要模版选项
/// </summary>
public class AbstractTemplateOptions
{
    /// <summary>
    /// 是否启用摘要
    /// </summary>
    public bool IsEnableAbstract { get; set; }

    /// <summary>
    /// 标题模版
    /// </summary>
    public List<NameValue> Templates { get; set; }
}