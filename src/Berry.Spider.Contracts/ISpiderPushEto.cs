namespace Berry.Spider;

public interface ISpiderPushEto : ISpiderEto
{
    /// <summary>
    /// 关键字
    /// </summary>
    string Keyword { get; set; }
    // List<string> Keywords { get; set; }
}