using Berry.Spider.Core;
using Berry.Spider.Core.Commands;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.AI.TextGeneration.Commands;

/// <summary>
/// 处理文件更改事件
/// </summary>
[CommandName(nameof(WatcherChangeTypes.Changed))]
public class FileOrFolderChangedCommand : IFixedCommand, ITransientDependency
{
    public async Task ExecuteAsync(CommandLineArgs commandLineArgs)
    {
        if (commandLineArgs.Body is FileSystemEventArgs e)
        {
            ConsoleHelper.WriteLine(@$"监听到新文件被创建: {e.FullPath}", ConsoleColor.Yellow);
        }
    }
}