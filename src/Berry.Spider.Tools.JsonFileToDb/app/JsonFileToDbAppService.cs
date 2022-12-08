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
    private SpiderDomainService SpiderDomainService { get; }
    private ILogger<JsonFileToDbAppService> Logger { get; }

    public JsonFileToDbAppService(ISpiderContentRepository spiderRepository, SpiderDomainService spiderDomainService,
        ILogger<JsonFileToDbAppService> logger)
    {
        SpiderRepository = spiderRepository;
        SpiderDomainService = spiderDomainService;
        Logger = logger;
    }

    public virtual async Task RunAsync()
    {
        string filePath = Path.Combine(AppContext.BaseDirectory, "files");
        // 递归获取文件路径下的所有文件
        var files = Directory.GetFiles(filePath, "*.json", SearchOption.AllDirectories);
        List<SpiderContent> spiderContents = new List<SpiderContent>();
        List<string> recommendTitleList = new();

        await Parallel.ForEachAsync(files, new ParallelOptions
        {
            MaxDegreeOfParallelism = GlobalConstants.ParallelMaxDegreeOfParallelism
        }, async (file, token) =>
        {
            try
            {
                if(file.Contains("(")) return;
                
                string fileContent = await File.ReadAllTextAsync(file, token);
                if (!string.IsNullOrEmpty(fileContent))
                {
                    JsonContentModel? jsonContentModel = JsonSerializer.Deserialize<JsonContentModel>(fileContent);
                    if (jsonContentModel != null)
                    {
                        recommendTitleList.AddRange(jsonContentModel.Pelates);
                        recommendTitleList.AddRange(jsonContentModel.Recommends);
                        //recommendTitleList.Distinct().ToList().RandomSort();

                        foreach (PostItem item in jsonContentModel.Posts)
                        {
                            List<string> contents = item.GetContentList();
                            if (contents.Count == 0) continue;

                            ListHelper listHelper = new ListHelper(contents);
                            List<string> todoSaveList = listHelper.GetList(20, 100);

                            if (todoSaveList.Count == 0) return;

                            var spiderContent = await SpiderDomainService.BuildContentAsync(jsonContentModel.keywords,
                                SpiderSourceFrom.Json_File_Import, todoSaveList);

                            // string realTitle = $"{jsonContentModel.keywords}{todoSaveList.Count}句";
                            // Console.WriteLine($"组合成功，新标题：{realTitle}");
                            //
                            // StringBuilder builder = new StringBuilder();
                            // for (int j = 0; j < todoSaveList.Count; j++)
                            // {
                            //     builder.AppendLine($"<p>{j + 1}、{todoSaveList[j].Trim()}</p>");
                            // }
                            //
                            // //组装数据
                            // var spiderContent = new SpiderContent(realTitle, builder.ToString(),
                            //     SpiderSourceFrom.Json_File_Import);

                            if (spiderContent != null)
                            {
                                spiderContents.Add(spiderContent);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        });

        //if (spiderContents.Count == 0) return;

        //保存推荐的标题
        string savePath = Path.Combine(filePath, DateTime.Now.ToString("yyyyMMdd"));
        string saveFileName = Path.Combine(savePath, $"{DateTime.Now.Ticks}.txt");
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }

        await File.WriteAllLinesAsync(saveFileName, recommendTitleList);

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