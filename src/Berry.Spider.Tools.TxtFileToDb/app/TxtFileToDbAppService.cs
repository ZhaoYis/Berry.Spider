using Berry.Spider.Domain;
using Berry.Spider.Domain.Shared;
using Microsoft.Extensions.Logging;

namespace Berry.Spider.Tools.TxtFileToDb.app;

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
    
    public async Task RunAsync()
    {
        string filePath = Path.Combine(AppContext.BaseDirectory, "files");
        // 递归获取文件路径下的所有文件
        var files = Directory.GetFiles(filePath, "*.txt", SearchOption.AllDirectories);
        List<SpiderContent> spiderContents = new List<SpiderContent>();
        foreach (string file in files)
        {
            string[] fileContents = await File.ReadAllLinesAsync(file);
            if (fileContents.Length > 2)
            {
                //第一条为标题
                string title = fileContents[0];
                //剩下部分为内容
                string content = string.Join("\n", fileContents.Skip(1));
                //组装数据
                var spiderContent = new SpiderContent(title, content, SpiderSourceFrom.TextFile_Import);
                spiderContents.Add(spiderContent);
            }
        }
        await this.SpiderRepository.InsertManyAsync(spiderContents);

        Logger.LogInformation($"导入成功，已导入：{spiderContents.Count}条");
    }
}