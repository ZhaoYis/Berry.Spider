using Berry.Spider.AI.TextGeneration.Storage;
using Berry.Spider.Core;
using Berry.Spider.Core.Commands;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.AI.TextGeneration.Commands;

/// <summary>
/// 处理文件重命名事件
/// </summary>
[CommandName(nameof(WatcherChangeTypes.Renamed))]
public sealed class FileOrFolderRenamedCommand : IFixedCommand, ITransientDependency
{
    public Task ExecuteAsync(CommandLineArgs commandLineArgs)
    {
        if (commandLineArgs.Body is RenamedEventArgs e)
        {
            ConsoleHelper.Info(@$"[变更]原始名称：{e.OldName}，新名称: {e.Name}");
            FileStorageProcessor.Instance.Remove(e.OldName ?? e.Name ?? e.FullPath);
            FileStorageProcessor.Instance.Add(e.Name ?? e.FullPath, e.FullPath);
        }

        return Task.CompletedTask;
    }
}