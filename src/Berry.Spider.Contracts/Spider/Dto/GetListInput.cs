using Volo.Abp.Application.Dtos;

namespace Berry.Spider;

public class GetListInput : PagedResultRequestDto
{
    /// <summary>
    /// 检索关键字
    /// </summary>
    public string Keyword { get; set; }
}