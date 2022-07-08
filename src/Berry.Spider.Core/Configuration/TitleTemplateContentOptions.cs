using Volo.Abp;

namespace Berry.Spider.Contracts;

/// <summary>
/// 文章标题模版选项
/// </summary>
public class TitleTemplateContentOptions
{
    /// <summary>
    /// 标题模版
    /// </summary>
    public List<NameValue?> Templates { get; set; }
}