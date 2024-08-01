using Berry.Spider.Core;
using Berry.Spider.Mmonly.Contracts;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using Volo.Abp.Application.Services;
using Volo.Abp.BackgroundJobs;

namespace Berry.Spider.Mmonly;

/// <summary>
/// https://www.mmonly.cc爬虫
/// </summary>
public class MmonlySpiderAppService : ApplicationService, IMmonlySpiderAppService
{
    private readonly IBackgroundJobManager _backgroundJobManager;
    private ILogger<MmonlySpiderAppService> Logger { get; }
    private IWebElementLoadProvider WebElementLoadProvider { get; }
    private IMmonlySourceProvider MmonlySourceProvider { get; }

    public MmonlySpiderAppService(IBackgroundJobManager backgroundJobManager,
        ILogger<MmonlySpiderAppService> logger,
        IWebElementLoadProvider webElementLoadProvider,
        IMmonlySourceProvider mmonlySourceProvider)
    {
        _backgroundJobManager = backgroundJobManager;
        Logger = logger;
        WebElementLoadProvider = webElementLoadProvider;
        MmonlySourceProvider = mmonlySourceProvider;
    }

    /// <summary>
    /// 下载https://www.mmonly.cc的图片资源
    /// </summary>
    /// <returns></returns>
    public async Task DownloadAsync()
    {
        //从一级页面拿到二级页面地址
        var urls = this.MmonlySourceProvider.GetUrls();
        urls.ToList().RandomSort();
        // await Parallel.ForEachAsync(urls, async (url,cancellationToken) =>
        // {
        //     this.Logger.LogInformation($"开始爬取{url}");
        //     
        //     await this.DownloadImageAsync(url);
        // });

        foreach (string url in urls)
        {
            this.Logger.LogInformation("开始爬取{Url}", url);

            await this.DownloadImageAsync(url);
        }
    }

    private async Task DownloadImageAsync(string targetUrl)
    {
        try
        {
            await this.WebElementLoadProvider.InvokeAsync(
                targetUrl, "",
                drv =>
                {
                    try
                    {
                        return drv.FindElement(By.Id("infinite_scroll"));
                    }
                    catch (Exception e)
                    {
                        return null;
                    }
                },
                async (root, keyword) =>
                {
                    if (root == null) return;

                    var resultContent = root.FindElements(By.CssSelector(".ABox"));
                    this.Logger.LogInformation("总共获取到记录：{Count}", resultContent.Count);

                    if (resultContent.Count > 0)
                    {
                        foreach (IWebElement element in resultContent)
                        {
                            var a = element.FindElement(By.TagName("a"));
                            if (a != null)
                            {
                                string href = a.GetAttribute("href");

                                //将二级页面地址推入后台job，执行具体的下载任务
                                await _backgroundJobManager.EnqueueAsync(
                                    new MmonlyFileDownloadArgs
                                    {
                                        TodoDownloadUrl = href
                                    }, BackgroundJobPriority.High, TimeSpan.FromSeconds(10)
                                );

                                this.Logger.LogInformation("获取到二级页面  ---> {Href}", href);
                            }
                        }
                    }
                });
        }
        catch (Exception exception)
        {
            this.Logger.LogException(exception);
        }
        finally
        {
            //ignore..
        }
    }
}