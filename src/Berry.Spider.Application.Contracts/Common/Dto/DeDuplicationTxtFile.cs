using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Berry.Spider.Common;

public class DeDuplicationTxtFile
{
    /// <summary>
    /// 文件
    /// </summary>
    [Required]
    public IFormFile File { get; set; }
}