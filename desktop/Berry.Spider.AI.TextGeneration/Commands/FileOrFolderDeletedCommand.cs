using Berry.Spider.AI.TextGeneration.Storage;
using Berry.Spider.Core;
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
            ConsoleHelper.WriteLine(@$"[删除]文件名称：{e.Name}，所在路径: {e.FullPath}", ConsoleColor.Red);
            FileStorageProcessor.Remove(e.Name ?? e.FullPath);
        }
    }
}