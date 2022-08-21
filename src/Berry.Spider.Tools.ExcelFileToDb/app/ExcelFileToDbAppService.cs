using System.Data;
using Berry.Spider.Core;
using Berry.Spider.Core.Helpers;
using Berry.Spider.Domain;
using Microsoft.Extensions.Logging;

namespace Berry.Spider.Tools.ExcelFileToDb;

/// <summary>
/// excel文件数据导入到MySQL
/// </summary>
public class ExcelFileToDbAppService : IExcelFileToDbAppService
{
    private ISpiderContentRepository SpiderRepository { get; }
    private ILogger<ExcelFileToDbAppService> Logger { get; }

    public ExcelFileToDbAppService(ISpiderContentRepository spiderRepository, ILogger<ExcelFileToDbAppService> logger)
    {
        SpiderRepository = spiderRepository;
        Logger = logger;
    }

    public virtual Task RunAsync()
    {
        string filePath = Path.Combine(AppContext.BaseDirectory, "files");
        // 递归获取文件路径下的所有文件
        var files = Directory.GetFiles(filePath, "*.xlsx", SearchOption.AllDirectories);
        List<SpiderContent> spiderContents = new List<SpiderContent>();
        foreach (string file in files)
        {
            DataTable? dt = OfficeHelper.ReadExcelToDataTable(file, isFirstRowColumn: false);
            if (dt != null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //组装数据
                    string title = dt.Rows[i][0].ToString();
                    string content = dt.Rows[i][1].ToString();
                    string keywords = dt.Rows[i][2].ToString();

                    if (!string.IsNullOrWhiteSpace(title) && !string.IsNullOrWhiteSpace(content))
                    {
                        var spiderContent = new SpiderContent(title, content, SpiderSourceFrom.Excel_File_Import, keywords: keywords, tag: keywords);
                        spiderContents.Add(spiderContent);
                    }
                }
            }
        }

        if (spiderContents.Count == 0) return Task.CompletedTask;

        int pageSize = 100;
        int pageIndex = 0;
        int totalCount = spiderContents.Count;
        int pageCount = (totalCount / pageSize) + (totalCount % pageSize > 0 ? 1 : 0);
        while (pageIndex < pageCount)
        {
            var spiderContentsPage = spiderContents.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            try
            {
                this.SpiderRepository.InsertManyAsync(spiderContentsPage, true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            pageIndex++;
        }

        return Task.CompletedTask;
    }
}