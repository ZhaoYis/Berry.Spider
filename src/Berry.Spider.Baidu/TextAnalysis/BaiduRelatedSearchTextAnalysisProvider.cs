using Berry.Spider.Core;

namespace Berry.Spider.Baidu;

/// <summary>
/// 百度相关推荐文本解析器
/// </summary>
public class BaiduRelatedSearchTextAnalysisProvider : ITextAnalysisProvider
{
    public Task<List<string>> InvokeAsync(string source)
    {
        return Task.FromResult(new List<string>());
    }
}