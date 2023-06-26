using System.ComponentModel.DataAnnotations;

namespace Berry.Spider.OpenAI.Contracts;

public class TextGenerationInput
{
    /// <summary>
    /// 关键字
    /// </summary>
    [Required]
    public string Keyword { get; set; }

    /// <summary>
    /// 最大长度。默认2048
    /// </summary>
    public int? MaxLength { get; set; } = 2048;
}