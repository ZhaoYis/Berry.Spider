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
[SpiderService(new[] {SpiderSourceFrom.Sogou_Related_Search})]
public class SogouSpider4RelatedSearchProvider : ProviderBase<SogouSpider4RelatedSearchProvider>, ISpiderProvider
{
    private IWebElementLoadProvider WebElementLoadProvider { get; }
    private ITextAnalysisProvider TextAnalysisProvider { get; }
    private IEventBusPublisher DistributedEventBus { get; }
    private IRedisService RedisService { get; }
    private ISpiderContentTitleRepository SpiderRepository { get; }
    private ISpiderContentKeywordRepository SpiderKeywordRepository { get; }
    private IOptionsSnapshot<SpiderOptions> Options { get; }

    private string HomePage => "https://sogou.com";

    public SogouSpider4RelatedSearchProvider(ILogger<SogouSpider4RelatedSearchProvider> logger,
        IWebElementLoadProvider provider,
        IServiceProvider serviceProvider,
        IEventBusPublisher eventBus,
        IRedisService redisService,
        ISpiderContentTitleRepository repository,
        ISpiderContentKeywordRepository keywordRepository,
        IOptionsSnapshot<SpiderOptions> options) : base(logger)
    {
        this.WebElementLoadProvider = provider;
        this.TextAnalysisProvider = serviceProvider.GetRequiredService<SogouRelatedSearchTextAnalysisProvider>();
        this.DistributedEventBus = eventBus;
        this.RedisService = redisService;
        this.SpiderRepository = repository;
        this.SpiderKeywordRepository = keywordRepository;
        this.Options = options;
    }

    /// <summary>
    /// 向队列推送源数据
    /// </summary>
    /// <returns></returns>
    public async Task PushAsync(string keyword, SpiderSourceFrom from)
    {
        SogouSpider4RelatedSearchPushEto push = new SogouSpider4RelatedSearchPushEto
        {
            SourceFrom = from,
            Keyword = keyword
        };

        await this.CheckAsync(push.Keyword, from,async () =>
            {
                await this.DistributedEventBus.PublishAsync(push.TryGetRoutingKey(), push);
            },
            bloomCheck: this.Options.Value.KeywordCheckOptions.BloomCheck,
            duplicateCheck: this.Options.Value.KeywordCheckOptions.RedisCheck);
    }

    /// <summary>
    /// 二次重复性校验
    /// </summary>
    /// <returns></returns>
    protected override async Task<bool> DuplicateCheckAsync(string keyword, SpiderSourceFrom from)
    {
        string key = GlobalConstants.SPIDER_KEYWORDS_KEY;
        if (this.Options.Value.KeywordCheckOptions.OnlyCurrentCategory)
        {
            key += $":{from.ToString()}";
        }

        bool result = await this.RedisService.SetAsync(key, keyword);
        return result;
    }

    /// <summary>
    /// 执行获取一级页面数据任务
    /// </summary>
    public async Task HandlePushEventAsync<T>(T eventData) where T : class, ISpiderPushEto
    {
        //获取url地址
        string realUrl = await this.WebElementLoadProvider.AutoClickAsync(this.HomePage, eventData.Keyword,
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
                if (resultContent is {Count: > 0})
                {
                    this.Logger.LogInformation("总共采集到记录：" + resultContent.Count);

                    var eto = new SogouSpider4RelatedSearchPullEto
                    {
                        Keyword = eventData.Keyword,
                        Title = eventData.Keyword
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

                            await Task.CompletedTask;
                        });

                    if (eto.Items.Any())
                    {
                        //此处不做消息队列发送，直接存储到数据库
                        await this.HandlePullEventAsync(eto);

                        //保存采集到的标题
                        List<SpiderContent_Keyword> list = eto.Items
                            .Select(item => new SpiderContent_Keyword(item.Title, eto.SourceFrom)).ToList();
                        await this.SpiderKeywordRepository.InsertManyAsync(list);
                    }
                }
            });
    }

    /// <summary>
    /// 执行根据一级页面采集到的地址获取二级页面具体目标数据任务
    /// </summary>
    public Task HandlePullEventAsync<T>(T eventData) where T : class, ISpiderPullEto
    {
        try
        {
            List<SpiderContent_Title> contents = new List<SpiderContent_Title>();
            foreach (var item in eventData.Items)
            {
                var content = new SpiderContent_Title(item.Title, item.Href, eventData.SourceFrom);
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