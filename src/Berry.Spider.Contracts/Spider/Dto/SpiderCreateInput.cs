using Berry.Spider.Domain.Shared;

namespace Berry.Spider;

public class SpiderCreateInput
{
    /// <summary>
    /// 爬虫名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 爬虫来源类型
    /// </summary>
    public SpiderSourceFrom From { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string Remark { get; set; }
}