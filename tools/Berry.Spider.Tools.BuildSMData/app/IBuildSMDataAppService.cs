using Volo.Abp.DependencyInjection;

namespace Berry.Spider.Tools.BuildSMData;

/// <summary>
/// 构建神马搜索数据
/// </summary>
public interface IBuildSMDataAppService : ITransientDependency
{
    Task RunAsync();
}