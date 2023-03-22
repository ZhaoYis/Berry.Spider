using Volo.Abp.DependencyInjection;

namespace Berry.Spider.Tools.ServDetector;

/// <summary>
/// 服务探测器
/// </summary>
public interface IServDetectorAppService : ITransientDependency
{
    Task RunAsync();
}