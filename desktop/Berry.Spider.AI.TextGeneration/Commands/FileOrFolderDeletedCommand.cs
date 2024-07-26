using Berry.Spider.Core.Commands;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.AI.TextGeneration.Commands;

/// <summary>
/// 处理文件删除事件
/// </summary>
[CommandName(nameof(WatcherChangeTypes.Deleted))]
public class FileOrFolderDeletedCommand : IFixedCommand, ITransientDependency
{
    public async Task ExecuteAsync(CommandLineArgs commandLineArgs)
    {
        if (commandLineArgs.Body is FileSystemEventArgs e)
        {
            Console.WriteLine(@$"File deleted: {e.FullPath}");
        }
    }
}