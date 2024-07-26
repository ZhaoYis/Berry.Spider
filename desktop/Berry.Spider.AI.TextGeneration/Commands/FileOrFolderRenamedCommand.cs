using Berry.Spider.Core.Commands;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.AI.TextGeneration.Commands;

/// <summary>
/// 处理文件重命名事件
/// </summary>
[CommandName(nameof(WatcherChangeTypes.Renamed))]
public class FileOrFolderRenamedCommand : IFixedCommand, ITransientDependency
{
    public async Task ExecuteAsync(CommandLineArgs commandLineArgs)
    {
        if (commandLineArgs.Body is RenamedEventArgs e)
        {
            Console.WriteLine(@$"File renamed: {e.OldName} to {e.Name}");
        }
    }
}