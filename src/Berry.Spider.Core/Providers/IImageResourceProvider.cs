using Volo.Abp.DependencyInjection;

namespace Berry.Spider.Core;

/// <summary>
/// 图片资源提供者
/// </summary>
public interface IImageResourceProvider : ISingletonDependency
{
    Task<string> TryGetAsync();
}