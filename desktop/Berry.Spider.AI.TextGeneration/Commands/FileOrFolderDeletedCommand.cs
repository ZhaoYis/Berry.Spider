using Berry.Spider.AI.TextGeneration.Storage;
using Berry.Spider.Core;
using Berry.Spider.Core.Commands;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.AI.TextGeneration.Commands;

/// <summary>
/// 处理文件删除事件
/// </summary>
[CommandName(nameof(WatcherChangeTypes.Deleted))]
public sealed class FileOrFolderDeletedCommand : IFixedCommand, ITransientDependency
{
    public Task ExecuteAsync(CommandLineArgs commandLineArgs)
    {
        if (commandLineArgs.Body is FileSystemEventArgs e)
        {
            ConsoleHelper.Info(@$"[删除]文件名称：{e.Name}，所在路径: {e.FullPath}");
            FileStorageProcessor.Instance.Remove(e.Name ?? e.FullPath);
        }

        return Task.CompletedTask;
    }
}