using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Berry.Spider.Common;

public class DeDuplicationTxtFile
{
    /// <summary>
    /// 文件
    /// </summary>
    [Required]
    public IFormFile File { get; set; }
}