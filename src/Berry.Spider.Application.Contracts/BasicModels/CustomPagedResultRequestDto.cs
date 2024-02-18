namespace Berry.Spider;

/// <summary>
/// 自定义分页请求参数
/// </summary>
public class CustomPagedResultRequestDto
{
    /// <summary>
    /// 当前页
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// 每页记录大小
    /// </summary>
    public int PageSize { get; set; }
}