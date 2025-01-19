using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

        var lines = await File.ReadAllLinesAsync(this.CurrentFilePath);
        if (lines is { Length: > 0 })
        {
            string saveFileName = $"{DateTime.Now:yyyyMMddHHmmssfff}.txt";
            string saveFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, saveFileName);

            await Parallel.ForEachAsync(lines, new ParallelOptions
            {
                MaxDegreeOfParallelism = AppGlobalConstants.ParallelSafeDegreeOfParallelism
            }, async (keyword, token) =>
            {
                if (string.IsNullOrEmpty(keyword)) return;
                this.SetExecLog($"开始抓取关键字：{keyword}");

                string targetUrl = string.Format(this.HomePage, keyword);
                await this.WebElementLoadProvider.InvokeAsync(targetUrl, keyword, drv => drv.FindElement(By.CssSelector(".s-result-list")), async (root, _keyword) =>
                {
                    if (root == null) return;
                    this.SetExecLog($"抓取到页面数据，开始执行数据解析操作...");

                    var resultContent = root.TryFindElements(By.CssSelector(".result-content"));
                    if (resultContent is null or { Count: 0 }) return;

                    var resultBag = new ConcurrentBag<ChildPageDataItem>();
                    await Parallel.ForEachAsync(resultContent, new ParallelOptions
                    {
                        MaxDegreeOfParallelism = AppGlobalConstants.ParallelMaxDegreeOfParallelism
                    }, async (element, _) =>
                    {
                        var a = element.TryFindElement(By.TagName("a"));
                        if (a != null)
                        {
                            string text = a.Text.Trim();
                            string href = a.GetDomAttribute("href");
                            string realHref = await this.ResolveJumpUrlProvider.ResolveAsync(href);

                            if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(realHref))
                            {
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
                        try
                        {
                            List<string> titles = resultBag.Select(x => x.Title).ToList();
                            //直接发送本地消息
                            await this.Mediator.Send(new ChildPageTitleRequest(saveFilePath, titles), token);
                            this.SetExecLog($"关键字：{keyword}，保存采集到的标题：{resultBag.Count}条{Environment.NewLine}");
                        }
                        catch (Exception e)
                        {
                            this.Logger.LogException(e);
                            this.SetExecLog($"[发生异常]{e.ToString()}");
                        }
                    }
                });

                await Task.Delay(1000, token);
            });
        }
    }

    private bool CanExecute()
    {
        return !string.IsNullOrEmpty(this.CurrentFilePath);
    }

    private static readonly StringBuilder logBuilder = new StringBuilder();

    private void SetExecLog(string message)
    {
        if (logBuilder.Length > 1_000_000) logBuilder.Clear();
        logBuilder.AppendFormat("{0:HH:mm:ss:fff} {1}{2}", DateTime.Now, message, Environment.NewLine);
        this.ExecLog = logBuilder.ToString();
    }
}