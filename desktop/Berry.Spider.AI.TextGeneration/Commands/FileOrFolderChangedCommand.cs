using Berry.Spider.Core;
using Berry.Spider.Core.Commands;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.AI.TextGeneration.Commands;

/// <summary>
/// 处理文件更改事件
/// </summary>
[CommandName(nameof(WatcherChangeTypes.Changed))]
public sealed class FileOrFolderChangedCommand : IFixedCommand, ITransientDependency
{
    public Task ExecuteAsync(CommandLineArgs commandLineArgs)
    {
        if (commandLineArgs.Body is FileSystemEventArgs e)
        {
            ConsoleHelper.Info(@$"监听到新文件被创建: {e.FullPath}");
        }

        return Task.CompletedTask;
    }
}