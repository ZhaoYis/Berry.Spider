using Berry.Spider.Core.Commands;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.AI.TextGeneration.Commands;

/// <summary>
/// 处理文件创建事件
/// </summary>
[CommandName(nameof(WatcherChangeTypes.Created))]
public class FileOrFolderCreatedCommand : IFixedCommand, ITransientDependency
{
    public async Task ExecuteAsync(CommandLineArgs commandLineArgs)
    {
        if (commandLineArgs.Body is FileSystemEventArgs e)
        {
            Console.WriteLine(@$"File created: {e.FullPath}");
        }
    }
}