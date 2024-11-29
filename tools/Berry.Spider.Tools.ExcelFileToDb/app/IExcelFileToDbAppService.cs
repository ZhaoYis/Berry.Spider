using Volo.Abp.DependencyInjection;

namespace Berry.Spider.Tools.ExcelFileToDb;

/// <summary>
/// Txt文件数据导入到MySQL
/// </summary>
public interface IExcelFileToDbAppService : ITransientDependency
{
    Task RunAsync();

    Task ExportToExcelAsync();
    
    /// <summary>
    /// 清理*.php文件并导出到excel
    /// </summary>
    /// <returns></returns>
    Task CleanAndExportToExcelAsync();
}