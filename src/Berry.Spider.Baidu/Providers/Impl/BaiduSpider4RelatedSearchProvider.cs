using System.Collections.Immutable;
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

namespace Berry.Spider.Baidu;

/// <summary>
/// 百度：相关推荐
/// </summary>
[SpiderService(new[] {SpiderSourceFrom.Baidu_Related_Search})]
public class BaiduSpider4RelatedSearchProvider : ProviderBase<BaiduSpider4RelatedSearchProvider>, ISpiderProvider
{
    private IWebElementLoadProvider WebElementLoadProvider { get; }
    private ITextAnalysisProvider TextAnalysisProvider { get; }
    private IResolveJumpUrlProvider ResolveJumpUrlProvider { get; }
    private IEventBusPublisher DistributedEventBus { get; }
    private IRedisService RedisService { get; }
    private ISpiderContentTitleRepository SpiderRepository { get; }
    private ISpiderContentKeywordRepository SpiderKeywordRepository { get; }
    private IOptionsSnapshot<SpiderOptions> Options { get; }

    private string HomePage => "https://www.baidu.com/s?wd={0}";

    public BaiduSpider4RelatedSearchProvider(ILogger<BaiduSpider4RelatedSearchProvider> logger,
        IWebElementLoadProvider provider,
        IServiceProvider serviceProvider,
        IEventBusPublisher eventBus,
        IRedisService redisService,
        ISpiderContentTitleRepository repository,
        ISpiderContentKeywordRepository keywordRepository,
        IOptionsSnapshot<SpiderOptions> options) : base(logger)
    {
        this.WebElementLoadProvider = provider;
        this.TextAnalysisProvider = serviceProvider.GetRequiredService<BaiduRelatedSearchTextAnalysisProvider>();
        this.ResolveJumpUrlProvider = serviceProvider.GetRequiredService<BaiduResolveJumpUrlProvider>();
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
        var eto = from.TryCreateEto(EtoType.Push, from, keyword);

        await this.CheckAsync(keyword, from, async () =>
            {
                string topicName = eto.TryGetRoutingKey();
                await this.DistributedEventBus.PublishAsync(topicName, eto);
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
        string targetUrl = string.Format(this.HomePage, eventData.Keyword);
        await this.WebElementLoadProvider.InvokeAsync(
            targetUrl,
            drv => drv.FindElement(By.Id("rs_new")),
            async root =>
            {
                if (root == null) return;

                var resultContent = root.TryFindElements(By.TagName("a"));
                if (resultContent is {Count: > 0})
                {
                    this.Logger.LogInformation("总共采集到记录：" + resultContent.Count);

                    ImmutableList<ChildPageDataItem> childPageDataItems = ImmutableList.Create<ChildPageDataItem>();
                    await Parallel.ForEachAsync(resultContent, new ParallelOptions
                    {
                        MaxDegreeOfParallelism = GlobalConstants.ParallelMaxDegreeOfParallelism
                    }, async (element, token) =>
                    {
                        string text = element.Text;
                        string href = element.GetAttribute("href");

                        //执行相似度检测
                        double sim = StringHelper.Sim(eventData.Keyword, text.Trim());
                        if (this.Options.Value.KeywordCheckOptions.IsEnableSimilarityCheck)
                        {
                            if (sim * 100 < this.Options.Value.KeywordCheckOptions.MinSimilarity)
                            {
                                return;
                            }
                        }

                        string realHref = await this.ResolveJumpUrlProvider.ResolveAsync(href);
                        if (!string.IsNullOrEmpty(realHref))
                        {
                            childPageDataItems = childPageDataItems.Add(new ChildPageDataItem
                            {
                                Title = text,
                                Href = realHref
                            });
                        }
                    });

                    if (childPageDataItems.Any())
                    {
                        var eto = eventData.SourceFrom.TryCreateEto(EtoType.Pull, eventData.SourceFrom, eventData.Keyword, eventData.Keyword, childPageDataItems.ToList());

                        //保存采集到的标题
                        if (eto is ISpiderPullEto pullEto)
                        {
                            //此处不做消息队列发送，直接存储到数据库
                            await this.HandlePullEventAsync(pullEto);

                            List<SpiderContent_Keyword> list = pullEto.Items.Select(item => new SpiderContent_Keyword(item.Title, pullEto.SourceFrom)).ToList();
                            await this.SpiderKeywordRepository.InsertManyAsync(list);
                        }
                    }
                }
            });
    }

    /// <summary>
    /// 执行根据一级页面采集到的地址获取二级页面具体目标数据任务
    /// </summary>
    public async Task HandlePullEventAsync<T>(T eventData) where T : class, ISpiderPullEto
    {
        try
        {
            List<SpiderContent_Title> contents = new List<SpiderContent_Title>();
            foreach (var item in eventData.Items)
            {
                var content = new SpiderContent_Title(item.Title, item.Href, eventData.SourceFrom);
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