using Berry.Spider.Abstractions;
using Berry.Spider.Contracts;
using Berry.Spider.Core;
using Berry.Spider.Domain;
using Berry.Spider.EventBus;
using Berry.Spider.FreeRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenQA.Selenium;

namespace Berry.Spider.Sogou;

/// <summary>
/// 搜狗：相关推荐
/// </summary>
[Spider(SpiderSourceFrom.Sogou_Related_Search)]
public class SogouSpider4RelatedSearchProvider : ProviderBase<SogouSpider4RelatedSearchProvider>, ISpiderProvider
{
    private IWebElementLoadProvider WebElementLoadProvider { get; }
    private ITextAnalysisProvider TextAnalysisProvider { get; }
    private IEventBusPublisher DistributedEventBus { get; }
    private IRedisService RedisService { get; }
    private ISpiderTitleContentRepository SpiderRepository { get; }
    private IOptionsSnapshot<SpiderOptions> Options { get; }

    private string HomePage => "https://sogou.com";

    public SogouSpider4RelatedSearchProvider(ILogger<SogouSpider4RelatedSearchProvider> logger,
        IWebElementLoadProvider provider,
        IServiceProvider serviceProvider,
        IEventBusPublisher eventBus,
        IRedisService redisService,
        ISpiderTitleContentRepository repository,
        IOptionsSnapshot<SpiderOptions> options) : base(logger)
    {
        this.WebElementLoadProvider = provider;
        this.TextAnalysisProvider = serviceProvider.GetRequiredService<SogouRelatedSearchTextAnalysisProvider>();
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
        await this.CheckAsync(push.Keyword, async () => { await this.DistributedEventBus.PublishAsync(push.TryGetRoutingKey(), push); },
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
            key += $":{SpiderSourceFrom.Sogou_Related_Search}";
        }

        bool result = await this.RedisService.SetAsync(key, keyword);
        return result;
    }

    /// <summary>
    /// 执行获取一级页面数据任务
    /// </summary>
    public async Task ExecuteAsync<T>(T request) where T : class, ISpiderRequest
    {
        //获取url地址
        string realUrl = await this.WebElementLoadProvider.AutoClickAsync(this.HomePage, request.Keyword,
            By.Id("query"),
            By.Id("stb"));
        if (string.IsNullOrWhiteSpace(realUrl)) return;

        await this.WebElementLoadProvider.InvokeAsync(
            realUrl,
            drv => drv.FindElement(By.Id("hint_container")),
            async root =>
            {
                if (root == null) return;

                var resultContent = root.TryFindElements(By.TagName("a"));
                if (resultContent is { Count: > 0 })
                {
                    this.Logger.LogInformation("总共获取到记录：" + resultContent.Count);

                    var eto = new SogouSpider4RelatedSearchPullEto
                    {
                        Keyword = request.Keyword,
                        Title = request.Keyword
                    };

                    await Parallel.ForEachAsync(resultContent, new ParallelOptions
                    {
                        MaxDegreeOfParallelism = GlobalConstants.ParallelMaxDegreeOfParallelism
                    },
                        async (element, token) =>
                        {
                            string text = element.Text;
                            string href = element.GetAttribute("href");

                            eto.Items.Add(new ChildPageDataItem
                            {
                                Title = text,
                                Href = href
                            });

                            this.Logger.LogInformation(text + "  ---> " + href);
                            await Task.CompletedTask;
                        });

                    if (eto.Items.Any())
                    {
                        //此处不做消息队列发送，直接存储到数据库
                        await this.HandleEventAsync(eto);
                        this.Logger.LogInformation("数据保存成功...");
                    }
                }
            });
    }

    /// <summary>
    /// 执行根据一级页面采集到的地址获取二级页面具体目标数据任务
    /// </summary>
    public Task HandleEventAsync<T>(T eventData) where T : class, ISpiderPullEto
    {
        try
        {
            List<SpiderTitleContent> contents = new List<SpiderTitleContent>();
            foreach (var item in eventData.Items)
            {
                var content = new SpiderTitleContent(item.Title, item.Href, eventData.SourceFrom);
                contents.Add(content);
            }

            return this.SpiderRepository.InsertManyAsync(contents);
        }
        catch (Exception exception)
        {
            this.Logger.LogException(exception);
        }

        return Task.CompletedTask;
    }
}