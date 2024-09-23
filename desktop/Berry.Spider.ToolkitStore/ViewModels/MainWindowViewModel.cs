using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Berry.Spider.ToolkitStore.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    /// <summary>
    /// 启动程序路径
    /// </summary>
    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(AppRunCommand))]
    private string? _appStrapPath;

    /// <summary>
    /// 程序是否运行中
    /// </summary>
    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(AppRunCommand))]
    private bool _isRunning = false;

    /// <summary>
    /// 选择启动程序
    /// </summary>
    [RelayCommand]
    private async Task AppSelectorAsync()
    {
        var desktop = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
        var storage = desktop?.MainWindow?.StorageProvider;
        if (storage is not null)
        {
            var files = await storage.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                AllowMultiple = false,
                FileTypeFilter = new[] { FilePickerFileTypes.TextPlain }
            });

            if (files is { Count: > 0 })
            {
                this.AppStrapPath = files[0].Path.ToString();
            }
        }

        await Task.CompletedTask;
    }

    /// <summary>
    /// 启动
    /// </summary>
    [RelayCommand(CanExecute = nameof(AppRunCanExecute))]
    private async Task AppRunAsync()
    {
        this.IsRunning = true;
        await Task.CompletedTask;
    }

    /// <summary>
    /// 取消
    /// </summary>
    [RelayCommand]
    private async Task AppCancleAsync()
    {
        this.IsRunning = false;
        await Task.CompletedTask;
    }

    /// <summary>
    /// 程序退出
    /// </summary>
    [RelayCommand]
    private async Task AppExitAsync()
    {
    }

    private bool AppRunCanExecute()
    {
        return this.IsRunning == false && this.AppStrapPath is not null;
    }
}