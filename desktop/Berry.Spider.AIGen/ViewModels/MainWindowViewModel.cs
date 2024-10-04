using System.Diagnostics;
using System.Timers;
using Berry.Spider.AIGen.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.AIGen.ViewModels;

public partial class MainWindowViewModel : ViewModelBase, IRecipient<NotifyTaskExecuteMessage>, ITransientDependency
{
    /// <summary>
    /// 应用名称
    /// </summary>
    [ObservableProperty] private string _applicationTitle = "Berry.Spider.AIGen";

    /// <summary>
    /// 主窗口提示信息
    /// </summary>
    [ObservableProperty] private string _tips = null!;

    private readonly Stopwatch _stopwatch;
    private readonly Timer _timer;

    public MainWindowViewModel()
    {
        _stopwatch = new Stopwatch();
        _timer = new Timer
        {
            Interval = 1000
        };
        WeakReferenceMessenger.Default.Register(this);
    }

    /// <summary>
    /// 处理通知消息
    /// </summary>
    public void Receive(NotifyTaskExecuteMessage message)
    {
        switch (message.IsRunning)
        {
            case true:
            {
                _timer.Elapsed += (sender, e) => { this.Tips = $"AI正在努力思考中，请稍后。当前已执行{_stopwatch.ElapsedMilliseconds / 1000}秒..."; };
                _stopwatch.Start();
                _timer.Start();
                break;
            }
            case false:
            {
                _timer.Stop();
                _stopwatch.Stop();
                _stopwatch.Reset();
                break;
            }
        }
    }
}