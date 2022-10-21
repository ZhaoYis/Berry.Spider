using Berry.Spider.Core;
using Berry.Spider.Domain;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Berry.Spider.Tools.TxtFileToDb;

/// <summary>
/// Txt文件数据导入到MySQL
/// </summary>
public class TxtFileToDbAppService : ITxtFileToDbAppService
{
    private ISpiderContentRepository SpiderRepository { get; }
    private ILogger<TxtFileToDbAppService> Logger { get; }

    public TxtFileToDbAppService(ISpiderContentRepository spiderRepository, ILogger<TxtFileToDbAppService> logger)
    {
        SpiderRepository = spiderRepository;
        Logger = logger;
    }

    public virtual async Task RunAsync()
    {
        string filePath = Path.Combine(AppContext.BaseDirectory, "files");
        // 递归获取文件路径下的所有文件
        var files = Directory.GetFiles(filePath, "*.txt", SearchOption.AllDirectories);
        List<SpiderContent> spiderContents = new List<SpiderContent>();
        foreach (string file in files)
        {
            string[] fileContents = File.ReadAllLines(file);
            StringBuilder builder = new StringBuilder();
            if (fileContents.Length > 2)
            {
                //第一条为标题
                string title = fileContents[0];
                //剩下部分为内容
                //string content = string.Join("\n", fileContents.Skip(1));
                foreach (var s in fileContents.Skip(1))
                {
                    builder.AppendFormat("<p>{0}</p>", s);
                }

                //组装数据
                var spiderContent = new SpiderContent(title, builder.ToString(), SpiderSourceFrom.Text_File_Import);
                spiderContents.Add(spiderContent);
            }
        }

        if (spiderContents.Count == 0) return;

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
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            pageIndex++;
        }
    }
}