namespace Berry.Spider.Contracts;

/// <summary>
/// 图片资源配置项
/// </summary>
public class ImageResourceOptions
{
    /// <summary>
    /// 图片资源地址模版
    /// </summary>
    public string TemplateUrl { get; set; }

    /// <summary>
    /// 资源最大ID
    /// </summary>
    public int MinId { get; set; }

    /// <summary>
    /// 资源最大ID
    /// </summary>
    public int MaxId { get; set; }
}