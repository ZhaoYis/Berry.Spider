using Berry.Spider.Abstractions;
using Berry.Spider.Core;
using Berry.Spider.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using System.Web;
using Volo.Abp.EventBus.Distributed;

namespace Berry.Spider.Baidu;

/// <summary>
/// 百度：相关推荐
/// </summary>
[Spider(SpiderSourceFrom.Baidu_Related_Search)]
public class BaiduSpider4RelatedSearchProvider : ProviderBase<BaiduSpider4RelatedSearchProvider>, ISpiderProvider
{
    private IWebElementLoadProvider WebElementLoadProvider { get; }
    private ITextAnalysisProvider TextAnalysisProvider { get; }
    private IDistributedEventBus DistributedEventBus { get; }
    private ISpiderTitleContentRepository SpiderRepository { get; }

    private string HomePage => "https://www.baidu.com/s?wd={0}";

    public BaiduSpider4RelatedSearchProvider(ILogger<BaiduSpider4RelatedSearchProvider> logger,
        IWebElementLoadProvider provider,
        IServiceProvider serviceProvider,
        IDistributedEventBus eventBus,
        ISpiderTitleContentRepository repository) : base(logger)
    {
        this.WebElementLoadProvider = provider;
        this.TextAnalysisProvider = serviceProvider.GetRequiredService<BaiduRelatedSearchTextAnalysisProvider>();
        this.DistributedEventBus = eventBus;
        this.SpiderRepository = repository;
    }

    /// <summary>
    /// 向队列推送源数据
    /// </summary>
    /// <returns></returns>
    public async Task PushAsync<T>(T push) where T : class, ISpiderPushEto
    {
        await this.BloomCheckAsync(push.Keyword, async () => { await this.DistributedEventBus.PublishAsync(push); });
    }

    /// <summary>
    /// 执行获取一级页面数据任务
    /// </summary>
    public async Task ExecuteAsync<T>(T request) where T : class, ISpiderRequest
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
                        var eto = new BaiduSpider4RelatedSearchPullEto
                            {Keyword = request.Keyword, Title = request.Keyword};

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
                            //await this.DistributedEventBus.PublishAsync(eto);

                            //此处不做消息队列发送，直接存储到数据库
                            await this.HandleEventAsync(eto);
                            this.Logger.LogInformation("数据保存成功...");
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
    public async Task HandleEventAsync<T>(T eventData) where T : class, ISpiderPullEto
    {
        try
        {
            List<SpiderTitleContent> contents = new List<SpiderTitleContent>();
            foreach (var item in eventData.Items)
            {
                var content = new SpiderTitleContent(item.Title, item.Href, eventData.SourceFrom);
                contents.Add(content);
            }

            await this.SpiderRepository.InsertManyAsync(contents);
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