using Berry.Spider.Core;

namespace Berry.Spider.Sogou
{
    /// <summary>
    /// 搜狗相关推荐文本解析器
    /// </summary>
    public class SogouRelatedSearchTextAnalysisProvider : ITextAnalysisProvider
    {
        public Task<List<string>> InvokeAsync(string source)
        {
            return Task.FromResult(new List<string>());
        }
    }
}