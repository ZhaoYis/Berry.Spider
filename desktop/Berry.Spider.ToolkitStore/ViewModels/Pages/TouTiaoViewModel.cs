using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Berry.Spider.Core;
using Berry.Spider.TouTiao;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.ToolkitStore.ViewModels.Pages;

public partial class TouTiaoViewModel : ViewModelBase, ITransientDependency
{
    /// <summary>
    /// 执行日志
    /// </summary>
    [ObservableProperty] private string _execLog;

    /// <summary>
    /// 当前选择的文件所在路径
    /// </summary>
    [NotifyCanExecuteChangedFor(nameof(ExecCommand))] [ObservableProperty]
    private string _currentFilePath;

    /// <summary>
    /// 是否正在执行
    /// </summary>
    [NotifyCanExecuteChangedFor(nameof(StopCommand), nameof(ExecCommand))] [ObservableProperty]
    private bool _isExecuting;

    private readonly CancellationTokenSource cancellationTokenSource = new();

    private IWebElementLoadProvider WebElementLoadProvider { get; }
    private IResolveJumpUrlProvider ResolveJumpUrlProvider { get; }
    private ILogger<TouTiaoViewModel> Logger { get; }
    private IMediator Mediator { get; }
    private string HomePage => "https://so.toutiao.com/search?keyword={0}&pd=question&dvpf=pc";

    public TouTiaoViewModel(IServiceProvider serviceProvider,
        IWebElementLoadProvider webProvider,
        ILogger<TouTiaoViewModel> logger,
        IMediator mediator)
    {
        this.WebElementLoadProvider = webProvider;
        this.ResolveJumpUrlProvider = serviceProvider.GetRequiredService<TouTiaoResolveJumpUrlProvider>();
        this.Logger = logger;
        this.Mediator = mediator;
    }

    /// <summary>
    /// 选择txt文件
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    private async Task ChoseFileAsync()
    {
        var sp = App.ResolveDefaultStorageProvider();
        if (sp is null) return;
        IReadOnlyList<IStorageFile> files = await sp.OpenFilePickerAsync(new FilePickerOpenOptions()
        {
            Title = "Open File",
            FileTypeFilter = new List<FilePickerFileType>
            {
                FilePickerFileTypes.TextPlain
            },
            AllowMultiple = false
        });

        if (files is { Count: > 0 })
        {
            IStorageFile file = files.First();
            this.CurrentFilePath = UrlHelper.UrlDecode(file.Path.AbsolutePath);
        }
    }

    /// <summary>
    /// 开始执行
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanExecute))]
    private async Task ExecAsync()
    {
        if (!File.Exists(this.CurrentFilePath)) return;

        // 设置正在执行状态
        this.IsExecuting = true;
        string saveFileName = $"{DateTime.Now:yyyyMMddHHmmssfff}.txt";
        string saveFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, saveFileName);

        IAsyncEnumerable<string> lines = File.ReadLinesAsync(this.CurrentFilePath, cancellationTokenSource.Token);
        Dictionary<string, string> keywordList = new Dictionary<string, string>(200_000);
        await foreach (var keyword in lines)
        {
            if (string.IsNullOrEmpty(keyword)) continue;
            string targetUrl = string.Format(this.HomePage, keyword);
            keywordList[keyword] = targetUrl;
        }

        await this.WebElementLoadProvider.BatchInvokeAsync(keywordList, drv => drv.FindElement(By.CssSelector(".s-result-list")), async (root, state) =>
        {
            if (root == null) return;

            var resultContent = root.TryFindElements(By.CssSelector(".result-content"));
            if (resultContent is null or { Count: 0 }) return;

            var resultBag = new ConcurrentBag<ChildPageDataItem>();
            await Parallel.ForEachAsync(resultContent, new ParallelOptions
            {
                MaxDegreeOfParallelism = AppGlobalConstants.ParallelMaxDegreeOfParallelism,
                CancellationToken = cancellationTokenSource.Token
            }, async (element, _innerToken) =>
            {
                var a = element.TryFindElement(By.TagName("a"));
                if (a != null)
                {
                    string text = a.Text.Trim();
                    string href = a.GetDomAttribute("href");

                    if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(href))
                    {
                        string realHref = await this.ResolveJumpUrlProvider.ResolveAsync(href);
                        resultBag.Add(new ChildPageDataItem
                        {
                            Title = text,
                            Href = realHref
                        });
                    }
                }
            });

            if (resultBag is { Count: > 0 })
            {
                List<string> titles = resultBag.Select(x => x.Title).ToList();
                //直接发送本地消息
                await this.Mediator.Send(new ChildPageTitleRequest(saveFilePath, titles), cancellationTokenSource.Token);
                this.SetExecLog($"关键字：{state}，保存采集到的标题：{resultBag.Count}条{Environment.NewLine}");
            }
        });
    }

    /// <summary>
    /// 停止执行
    /// </summary>
    [RelayCommand(CanExecute = nameof(IsExecuting))]
    private async Task StopAsync()
    {
        await this.cancellationTokenSource.CancelAsync();
        this.IsExecuting = false;
    }

    /// <summary>
    /// 是否可以点击去执行按钮
    /// </summary>
    /// <returns></returns>
    private bool CanExecute()
    {
        return !string.IsNullOrEmpty(this.CurrentFilePath) && !this.IsExecuting;
    }

    private static readonly StringBuilder logBuilder = new StringBuilder();

    private void SetExecLog(string message)
    {
        if (logBuilder.Length > 1_000_000) logBuilder.Clear();
        logBuilder.AppendFormat("{0:HH:mm:ss:fff} {1}{2}", DateTime.Now, message, Environment.NewLine);
        this.ExecLog = logBuilder.ToString();
    }
}