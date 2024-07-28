using System.Text;
using Berry.Spider.AI.TextGeneration.Storage;
using Berry.Spider.Core;
using Berry.Spider.Core.Commands;
using Berry.Spider.Domain;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.TextGeneration;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.AI.TextGeneration.Commands;

/// <summary>
/// 处理文件创建事件
/// </summary>
[CommandName(nameof(WatcherChangeTypes.Created))]
public class FileOrFolderCreatedCommand : IFixedCommand, ITransientDependency
{
    private ITextGenerationService TextGenerationService { get; }
    private ISpiderContentRepository SpiderRepository { get; }

    public FileOrFolderCreatedCommand(Kernel kernel, ISpiderContentRepository repository)
    {
        this.TextGenerationService = kernel.GetRequiredService<ITextGenerationService>();
        this.SpiderRepository = repository;
    }

    public async Task ExecuteAsync(CommandLineArgs commandLineArgs)
    {
        if (commandLineArgs.Body is FileSystemEventArgs e)
        {
            ConsoleHelper.WriteLine(@$"[新增]文件名称：{e.Name}，所在路径: {e.FullPath}", ConsoleColor.Green);
            FileStorageProcessor.Add(e.Name ?? e.FullPath, e.FullPath);

            //TODO：通知程序开始处理文件内容
            await foreach (var line in File.ReadLinesAsync(Path.Combine(AppContext.BaseDirectory, e.FullPath)))
            {
                string originalTitle = line.Trim();

                ConsoleHelper.WriteLine($"开始生成标题为[{originalTitle}]的文章...");
                StringBuilder article = new StringBuilder();
                await foreach (var text in this.TextGenerationService.GetStreamingTextContentsAsync(originalTitle))
                {
                    article.Append(text.Text);
                    ConsoleHelper.Write(text.Text ?? "", ConsoleColor.Magenta);
                }

                await this.SaveArticleAsync(originalTitle, article.ToString(), e.Name ?? e.FullPath);
                await Task.Delay(50);
            }
        }
    }

    private async Task SaveArticleAsync(string title, string article, string traceCode)
    {
        try
        {
            var content = new SpiderContent(title, article.ToString(), SpiderSourceFrom.AI);
            content.SetTraceCodeIfNotNull(traceCode);

            await this.SpiderRepository.InsertAsync(content);
            ConsoleHelper.WriteLine($"标题为[{title}]的文章保存成功...");
        }
        catch (Exception e)
        {
            ConsoleHelper.WriteLine(e.ToString(), ConsoleColor.Red);
        }
    }
}