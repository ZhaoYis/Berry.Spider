using Volo.Abp.DependencyInjection;

namespace Berry.Spider.Tools.JsonFileToDb;

/// <summary>
/// Json文件数据导入到MySQL
/// </summary>
public interface IJsonFileToDbAppService : ITransientDependency
{
    Task RunAsync();
}