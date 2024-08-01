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
public sealed class FileOrFolderCreatedCommand(Kernel kernel, ISpiderContentRepository repository) : IFixedCommand, ITransientDependency
{
    private ITextGenerationService TextGenerationService { get; } = kernel.GetRequiredService<ITextGenerationService>();
    private ISpiderContentRepository SpiderRepository { get; } = repository;

    public async Task ExecuteAsync(CommandLineArgs commandLineArgs)
    {
        if (commandLineArgs.Body is FileSystemEventArgs e)
        {
            ConsoleHelper.Info(@$"[新增]文件名称：{e.Name}，所在路径: {e.FullPath}");
            FileStorageProcessor.Instance.Add(e.Name ?? e.FullPath, e.FullPath);

            //TODO：通知程序开始处理文件内容
            await foreach (var line in File.ReadLinesAsync(Path.Combine(AppContext.BaseDirectory, e.FullPath)))
            {
                string originalTitle = line.Trim();

                ConsoleHelper.Info($"开始生成标题为[{originalTitle}]的文章...");
                StringBuilder article = new StringBuilder();
                await foreach (var text in this.TextGenerationService.GetStreamingTextContentsAsync(originalTitle))
                {
                    article.Append(text.Text);
                    ConsoleHelper.Info(text.Text ?? "");
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
            var content = new SpiderContent(title, article, SpiderSourceFrom.AI);
            content.SetTraceCodeIfNotNull(traceCode);

            await this.SpiderRepository.InsertAsync(content);
            ConsoleHelper.Info($"标题为[{title}]的文章保存成功...");
        }
        catch (Exception e)
        {
            ConsoleHelper.Error(e.ToString());
            ConsoleHelper.Error($"---------------错误标题：{title}---------------");
        }
    }
}