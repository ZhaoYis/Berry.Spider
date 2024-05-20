namespace Berry.Spider.NaiPan;

public interface INaiPanService
{
    /// <summary>
    /// 生成伪原创内容
    /// </summary>
    /// <returns></returns>
    Task<string> GenerateAsync(string content);
}