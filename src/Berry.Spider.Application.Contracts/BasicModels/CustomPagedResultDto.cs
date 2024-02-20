using Volo.Abp.Application.Dtos;

namespace Berry.Spider;

/// <summary>
/// 自定义分页结果集
/// </summary>
[Serializable]
public class CustomPagedResultDto<T> : ListResultDto<T>
{
    /// <summary>
    /// 总记录数
    /// </summary>
    public long Total { get; set; }

    public CustomPagedResultDto()
    {

    }

    public CustomPagedResultDto(long totalCount, IReadOnlyList<T> items) : base(items)
    {
        Total = totalCount;
    }
}