namespace Berry.Spider.Contracts;

public interface ISpiderRequest
{
    /// <summary>
    /// 检索关键字
    /// </summary>
    string Keyword { get; set; }
}