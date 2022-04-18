using System.Web;
using Berry.Spider.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using Volo.Abp.EventBus.Distributed;

namespace Berry.Spider.Baidu;

/// <summary>
/// 百度：相关推荐
/// </summary>
public class BaiduSpider4RelatedSearchProvider : IBaiduSpiderProvider
{
    private ILogger<BaiduSpider4RelatedSearchProvider> Logger { get; }
    private IWebElementLoadProvider WebElementLoadProvider { get; }
    private ITextAnalysisProvider TextAnalysisProvider { get; }
    private IDistributedEventBus DistributedEventBus { get; }

    private string HomePage => "https://www.baidu.com/s?wd={0}";

    public BaiduSpider4RelatedSearchProvider(ILogger<BaiduSpider4RelatedSearchProvider> logger,
        IWebElementLoadProvider provider,
        IServiceProvider serviceProvider,
        IDistributedEventBus eventBus)
    {
        this.Logger = logger;
        this.WebElementLoadProvider = provider;
        this.TextAnalysisProvider = serviceProvider.GetRequiredService<BaiduRelatedSearchTextAnalysisProvider>();
        this.DistributedEventBus = eventBus;
    }

    /// <summary>
    /// 执行获取一级页面数据任务
    /// </summary>
    public async Task ExecuteAsync<T>(T request) where T : ISpiderRequest
    {
        try
        {
            string targetUrl = string.Format(this.HomePage, request.Keyword);
            await this.WebElementLoadProvider.InvokeAsync(
                targetUrl,
                drv =>
                {
                    try
                    {
                        // return drv.FindElement(By.CssSelector(".result-molecule"));
                        return drv.FindElement(By.Id("rs_new"));
                    }
                    catch (Exception e)
                    {
                        return null;
                    }
                },
                async root =>
                {
                    if (root == null) return;

                    var resultContent = root.FindElements(By.TagName("a"));
                    this.Logger.LogInformation("总共获取到记录：" + resultContent.Count);

                    if (resultContent.Count > 0)
                    {
                        var eto = new BaiduSpider4RelatedSearchEto {Keyword = request.Keyword, Title = request.Keyword};

                        foreach (IWebElement element in resultContent)
                        {
                            string text = element.Text;
                            string href = element.GetAttribute("href");

                            if (href.StartsWith("http") || href.StartsWith("https"))
                            {
                                Uri jumpUri = new Uri(HttpUtility.UrlDecode(href));
                                if (jumpUri.Host.Contains("baidu"))
                                {
                                    eto.Items.Add(new ChildPageDataItem
                                    {
                                        Title = text,
                                        Href = jumpUri.ToString()
                                    });

                                    this.Logger.LogInformation(text + "  ---> " + href);
                                }
                            }
                        }

                        if (eto.Items.Any())
                        {
                            await this.DistributedEventBus.PublishAsync(eto);
                            this.Logger.LogInformation("事件发布成功，等待消费...");
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

    /// <summary>
    /// 执行根据一级页面采集到的地址获取二级页面具体目标数据任务
    /// </summary>
    public async Task HandleEventAsync<T>(T eventData) where T : ISpiderPullEto
    {
        try
        {
            //TODO:入库操作
            await Task.CompletedTask;
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