using Berry.Spider.Core.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Volo.Abp.Threading;

namespace Berry.Spider.AI.TextGeneration;

public class FileWatcherHostedService : IHostedService
{
    private readonly FileSystemWatcher _fileSystemWatcher;
    private IServiceScopeFactory ServiceScopeFactory { get; }
    private ICommandSelector CommandSelector { get; }

    public FileWatcherHostedService(IServiceScopeFactory factory, ICommandSelector commandSelector)
    {
        _fileSystemWatcher = new FileSystemWatcher(Path.Combine("files"));
        this.ServiceScopeFactory = factory;
        this.CommandSelector = commandSelector;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        //监视最近写入、目录更改和文件名更改
        _fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.DirectoryName | NotifyFilters.FileName;
        //过滤后缀文件
        _fileSystemWatcher.Filter = "*.txt";
        _fileSystemWatcher.Created += OnFileCreated;
        _fileSystemWatcher.Changed += OnFileChanged;
        _fileSystemWatcher.Deleted += OnFileDeleted;
        _fileSystemWatcher.Renamed += OnFileRenamed;
        _fileSystemWatcher.Error += OnError;
        //是否包含子文件夹
        _fileSystemWatcher.IncludeSubdirectories = true;
        //是否启用事件监听
        _fileSystemWatcher.EnableRaisingEvents = true;
        await Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        // 在一段时间后或不再需要时停止监视
        _fileSystemWatcher.Dispose();
        await Task.CompletedTask;
    }

    private void OnFileCreated(object sender, FileSystemEventArgs e)
    {
        var (command, commandLineArgs) = this.GetCommand(nameof(WatcherChangeTypes.Created), e);
        AsyncHelper.RunSync(() => command.ExecuteAsync(commandLineArgs));
    }

    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        var (command, commandLineArgs) = this.GetCommand(nameof(WatcherChangeTypes.Changed), e);
        AsyncHelper.RunSync(() => command.ExecuteAsync(commandLineArgs));
    }

    private void OnFileDeleted(object sender, FileSystemEventArgs e)
    {
        var (command, commandLineArgs) = this.GetCommand(nameof(WatcherChangeTypes.Deleted), e);
        AsyncHelper.RunSync(() => command.ExecuteAsync(commandLineArgs));
    }

    private void OnFileRenamed(object sender, RenamedEventArgs e)
    {
        var (command, commandLineArgs) = this.GetCommand(nameof(WatcherChangeTypes.Renamed), e);
        AsyncHelper.RunSync(() => command.ExecuteAsync(commandLineArgs));
    }
    
    private void OnError(object sender, ErrorEventArgs e)
    {
        // 错误消息
        Console.WriteLine(@$"{e.GetException().ToString()}");
    }

    private (IFixedCommand, CommandLineArgs) GetCommand(string commandName, object e)
    {
        using var scope = ServiceScopeFactory.CreateScope();
        CommandLineArgs commandLineArgs = new CommandLineArgs(commandName, e);
        var commandType = this.CommandSelector.Select(commandLineArgs);
        var command = (IFixedCommand)scope.ServiceProvider.GetRequiredService(commandType);
        return (command, commandLineArgs);
    }
}