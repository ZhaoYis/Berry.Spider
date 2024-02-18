using Berry.Spider.Core;

namespace Berry.Spider;

public class CustomGetListInput : CustomPagedResultRequestDto
{
    /// <summary>
    /// 检索关键字
    /// </summary>
    public string? Keyword { get; set; }
}