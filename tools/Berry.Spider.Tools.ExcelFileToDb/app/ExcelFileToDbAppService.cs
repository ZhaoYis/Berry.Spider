using Berry.Spider.Core;
using Berry.Spider.Core.Helpers;
using Berry.Spider.Domain;
using Microsoft.Extensions.Logging;
using System.Data;

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

    public virtual async Task RunAsync()
    {
        string filePath = Path.Combine(AppContext.BaseDirectory, "files");
        // 递归获取文件路径下的所有文件
        var files = Directory.GetFiles(filePath, "*.xlsx", SearchOption.AllDirectories);
        List<SpiderContent> spiderContents = new List<SpiderContent>();
        foreach (string file in files)
        {
            DataTable? dt = OfficeHelper.ReadExcelToDataTable(file, isFirstRowColumn: true);
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

        if (spiderContents.Count == 0) return;

        // //清理相似度比较高的记录
        // List<SpiderContent> currentList = JsonConvert.DeserializeObject<List<SpiderContent>>(JsonConvert.SerializeObject(spiderContents));
        // foreach (SpiderContent content in spiderContents)
        // {
        //     for (int i = currentList.Count - 1; i >= 0; i--)
        //     {
        //         SpiderContent current = currentList[i];
        //
        //         var sim = StringHelper.Sim(content.Title, current.Title);
        //         if (sim >= 0.8 && sim - 1 != 0)
        //         {
        //             Console.WriteLine($"标题1：{content.Title}，标题2：{current.Title}，相似度：{sim * 100:F}%");
        //             currentList.RemoveAt(i);
        //         }
        //     }
        // }

        int pageSize = 100;
        int pageIndex = 0;
        int totalCount = spiderContents.Count;
        int pageCount = (totalCount / pageSize) + (totalCount % pageSize > 0 ? 1 : 0);
        while (pageIndex < pageCount)
        {
            var spiderContentsPage = spiderContents.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            try
            {
                await this.SpiderRepository.InsertManyAsync(spiderContentsPage, true);
                await Task.Delay(2);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            pageIndex++;
        }
    }
}