using Berry.Spider.Core;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using Berry.Spider.Domain.Shared;

namespace Berry.Spider;

public class PushFromFileBasicDto : ITraceCode
{
    private string? _traceCode;

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

    /// <summary>
    /// 用作当前关键字最初导入时的批次追踪标识
    /// </summary>
    public string? TraceCode
    {
        get
        {
            string? fileName = this.File.FileName[0..this.File.FileName.LastIndexOf('.')];
            if (string.IsNullOrEmpty(_traceCode) && !string.IsNullOrWhiteSpace(fileName))
            {
                _traceCode = fileName;
            }

            return _traceCode;
        }
        set => _traceCode = value;
    }
}