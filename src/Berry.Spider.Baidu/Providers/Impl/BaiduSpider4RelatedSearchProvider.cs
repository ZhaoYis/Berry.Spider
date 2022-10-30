using Berry.Spider.Abstractions;
using Berry.Spider.Core;
using Berry.Spider.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using System.Web;
using Berry.Spider.Contracts;
using Berry.Spider.FreeRedis;
using Microsoft.Extensions.Options;
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
    private IRedisService RedisService { get; }
    private ISpiderTitleContentRepository SpiderRepository { get; }
    private IOptionsSnapshot<SpiderOptions> Options { get; }

    private string HomePage => "https://www.baidu.com/s?wd={0}";

    public BaiduSpider4RelatedSearchProvider(ILogger<BaiduSpider4RelatedSearchProvider> logger,
        IWebElementLoadProvider provider,
        IServiceProvider serviceProvider,
        IDistributedEventBus eventBus,
        IRedisService redisService,
        ISpiderTitleContentRepository repository,
        IOptionsSnapshot<SpiderOptions> options) : base(logger)
    {
        this.WebElementLoadProvider = provider;
        this.TextAnalysisProvider = serviceProvider.GetRequiredService<BaiduRelatedSearchTextAnalysisProvider>();
        this.DistributedEventBus = eventBus;
        this.RedisService = redisService;
        this.SpiderRepository = repository;
        this.Options = options;
    }

    /// <summary>
    /// 向队列推送源数据
    /// </summary>
    /// <returns></returns>
    public async Task PushAsync<T>(T push) where T : class, ISpiderPushEto
    {
        await this.CheckAsync(push.Keyword, async () => { await this.DistributedEventBus.PublishAsync(push); },
            bloomCheck: this.Options.Value.KeywordCheckOptions.BloomCheck,
            duplicateCheck: this.Options.Value.KeywordCheckOptions.RedisCheck);
    }

    /// <summary>
    /// 二次重复性校验
    /// </summary>
    /// <returns></returns>
    protected override async Task<bool> DuplicateCheckAsync(string keyword)
    {
        string key = GlobalConstants.SPIDER_KEYWORDS_KEY;
        if (this.Options.Value.KeywordCheckOptions.OnlyCurrentCategory)
        {
            key += $":{SpiderSourceFrom.Baidu_Related_Search}";
        }

        bool result = await this.RedisService.SetAsync(key, keyword);
        return result;
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
                drv => drv.FindElement(By.Id("rs_new")),
                async root =>
                {
                    if (root == null) return;

                    var resultContent = root.FindElements(By.TagName("a"));
                    this.Logger.LogInformation("总共获取到记录：" + resultContent.Count);

                    if (resultContent.Count > 0)
                    {
                        var eto = new BaiduSpider4RelatedSearchPullEto
                        {
                            Keyword = request.Keyword,
                            Title = request.Keyword
                        };

                        await Parallel.ForEachAsync(resultContent, new ParallelOptions
                        {
                            MaxDegreeOfParallelism = 10
                        }, async (element, token) =>
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

                            await Task.CompletedTask;
                        });

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
    }
}