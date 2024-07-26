using Microsoft.Extensions.Hosting;

namespace Berry.Spider.AI.TextGeneration;

public class FileWatcherHostedService : IHostedService
{
    private readonly FileSystemWatcher _fileSystemWatcher;

    public FileWatcherHostedService()
    {
        _fileSystemWatcher = new FileSystemWatcher(Path.Combine("files"));
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
        // 处理文件创建事件
        Console.WriteLine($"File created: {e.FullPath}");
    }

    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        // 处理文件更改事件
        Console.WriteLine($"File changed: {e.FullPath}");
    }

    private void OnFileDeleted(object sender, FileSystemEventArgs e)
    {
        // 处理文件删除事件
        Console.WriteLine($"File deleted: {e.FullPath}");
    }

    private void OnFileRenamed(object sender, RenamedEventArgs e)
    {
        // 处理文件重命名事件
        Console.WriteLine($"File renamed: {e.OldName} to {e.Name}");
    }

    private void OnError(object sender, ErrorEventArgs e)
    {
        // 错误消息
        Console.WriteLine($"{e.GetException().ToString()}");
    }
}