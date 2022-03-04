using Volo.Abp.DependencyInjection;

namespace Berry.Spider.Core;

/// <summary>
/// 内容解析器
/// </summary>
public interface ITextAnalysisProvider : ITransientDependency
{
    Task<List<string>> InvokeAsync(string source);
}