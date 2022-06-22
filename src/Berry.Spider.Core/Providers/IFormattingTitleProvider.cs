using Volo.Abp.DependencyInjection;

namespace Berry.Spider.Core;

/// <summary>
/// 标题格式化器
/// </summary>
public interface IFormattingTitleProvider : ISingletonDependency
{
    /// <summary>
    /// 按照配置规则格式化标题
    /// </summary>
    /// <returns></returns>
    string Format(string title, int total);
}