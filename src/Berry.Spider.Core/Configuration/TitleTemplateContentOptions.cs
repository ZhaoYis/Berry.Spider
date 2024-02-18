using Volo.Abp;

namespace Berry.Spider.Core;

/// <summary>
/// 文章标题模版选项
/// </summary>
public class TitleTemplateContentOptions
{
    /// <summary>
    /// 是否启用格式化标题
    /// </summary>
    public bool IsEnableFormatTitle { get; set; }

    /// <summary>
    /// 标题模版
    /// </summary>
    public List<NameValue> Templates { get; set; } = new();
}