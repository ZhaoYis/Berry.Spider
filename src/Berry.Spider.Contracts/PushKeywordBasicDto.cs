using System.ComponentModel.DataAnnotations;

namespace Berry.Spider;

public class PushKeywordBasicDto
{
    /// <summary>
    /// 关键字
    /// </summary>
    [Required]
    public string Keyword { get; set; } = null!;
}