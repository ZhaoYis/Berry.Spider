using Volo.Abp.DependencyInjection;

namespace Berry.Spider.Tools.TxtFileToDb;

/// <summary>
/// Txt文件数据导入到MySQL
/// </summary>
public interface ITxtFileToDbAppService : ITransientDependency
{
    Task RunAsync();
}