using System.Text;
using System.Text.Json;
using Berry.Spider.Core;
using Berry.Spider.Domain;
using Microsoft.Extensions.Logging;

namespace Berry.Spider.Tools.JsonFileToDb;

/// <summary>
/// Json文件数据导入到MySQL
/// </summary>
public class JsonFileToDbAppService : IJsonFileToDbAppService
{
    private ISpiderContentRepository SpiderRepository { get; }
    private ILogger<JsonFileToDbAppService> Logger { get; }

    public JsonFileToDbAppService(ISpiderContentRepository spiderRepository, ILogger<JsonFileToDbAppService> logger)
    {
        SpiderRepository = spiderRepository;
        Logger = logger;
    }

    public virtual async Task RunAsync()
    {
        string filePath = Path.Combine(AppContext.BaseDirectory, "files");
        // 递归获取文件路径下的所有文件
        var files = Directory.GetFiles(filePath, "*.json", SearchOption.AllDirectories);
        List<SpiderContent> spiderContents = new List<SpiderContent>();
        foreach (string file in files)
        {
            string fileContent = await File.ReadAllTextAsync(file);
            if (!string.IsNullOrEmpty(fileContent))
            {
                JsonContentModel? jsonContentModel = JsonSerializer.Deserialize<JsonContentModel>(fileContent);
                if (jsonContentModel != null)
                {
                    List<string> titles = new();
                    const int maxPageSize = 50;
                    titles.AddRange(jsonContentModel.Pelates);
                    titles.AddRange(jsonContentModel.Recommends);
                    titles.RandomSort();

                    await Parallel.ForEachAsync(jsonContentModel.Posts, new ParallelOptions
                    {
                        MaxDegreeOfParallelism = GlobalConstants.ParallelMaxDegreeOfParallelism
                    }, async (item, token) =>
                    {
                        List<string> contents = item.GetContentList();
                        for (int i = 0; i < titles.Count; i++)
                        {
                            List<string> todoSaveList;
                            string title = titles[i];

                            if ((i + 1) * maxPageSize <= contents.Count)
                            {
                                todoSaveList = contents.Skip(i * maxPageSize).Take(maxPageSize).ToList();
                            }
                            else
                            {
                                todoSaveList = contents.Skip(i * maxPageSize)
                                    .Take(contents.Count - (i * maxPageSize)).ToList();
                            }

                            if (todoSaveList.Count == 0) return;

                            StringBuilder builder = new StringBuilder();
                            for (int j = 0; j < todoSaveList.Count; j++)
                            {
                                builder.AppendLine($"<p>{j + 1}、{todoSaveList[j]}</p>");
                            }

                            //组装数据
                            var spiderContent = new SpiderContent(title, builder.ToString(),
                                SpiderSourceFrom.Json_File_Import);
                            spiderContents.Add(spiderContent);
                        }

                        await Task.CompletedTask;
                    });
                }
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