using System.ComponentModel.DataAnnotations;
using Berry.Spider.Core;
using Microsoft.AspNetCore.Http;

namespace Berry.Spider;

public class PushFromFileBasicDto
{
    /// <summary>
    /// 来源
    /// </summary>
    [Required]
    public SpiderSourceFrom SourceFrom { get; set; }

    /// <summary>
    /// 文件
    /// </summary>
    [Required]
    public IFormFile File { get; set; }
}