namespace Berry.Spider.Core;

/// <summary>
/// 内容解析器
/// </summary>
public interface ITextAnalysisProvider
{
    Task<List<string>> InvokeAsync(string source);
}