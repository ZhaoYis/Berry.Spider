namespace Berry.Spider.Segmenter;

public interface ISegmenterProvider
{
    /// <summary>
    /// 中文分词(搜索引擎模式)
    /// </summary>
    /// <returns></returns>
    Task<List<string>> CutForSearchAsync(string source);
}