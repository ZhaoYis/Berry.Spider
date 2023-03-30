namespace Berry.Spider;

public interface ISpiderPullEto : ISpiderEto
{
    /// <summary>
    /// 保存这次记录最终的标题
    /// </summary>
    string Title { get; set; }

    /// <summary>
    /// 关键字
    /// </summary>
    string Keyword { get; set; }

    /// <summary>
    /// 二级页面地址信息
    /// </summary>
    List<ChildPageDataItem> Items { get; set; }
}