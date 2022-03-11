namespace Berry.Spider;

public interface ISpiderPushEto : ISpiderEto
{
    /// <summary>
    /// 关键字
    /// </summary>
    List<string> Keywords { get; set; }
}