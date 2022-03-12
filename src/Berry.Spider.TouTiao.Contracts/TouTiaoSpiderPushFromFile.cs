using System.ComponentModel.DataAnnotations;
using Berry.Spider.Domain.Shared;
using Microsoft.AspNetCore.Http;

namespace Berry.Spider.TouTiao;

public class TouTiaoSpiderPushFromFile
{
    /// <summary>
    /// 来源
    /// </summary>
    public SpiderSourceFrom SourceFrom { get; set; }

    /// <summary>
    /// 文件
    /// </summary>
    [Required]
    public IFormFile File { get; set; }
}