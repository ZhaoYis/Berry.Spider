using System.Collections.Immutable;
using Berry.Spider.Application.Contracts;
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
    private SpiderOptions Options { get; }

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
        this.Options = options.Value;
    }

    /// <summary>
    /// 向队列推送源数据
    /// </summary>
    /// <returns></returns>
    public async Task PushAsync(SpiderPushToQueueDto dto)
    {
        string identityId = dto.GetIdentityId();
        var eto = dto.SourceFrom.TryCreateEto(EtoType.Push, dto.SourceFrom, dto.Keyword, dto.TraceCode, identityId);

        await this.CheckAsync(identityId, dto.SourceFrom, async () =>
            {
                string topicName = eto.TryGetRoutingKey();
                await this.DistributedEventBus.PublishAsync(topicName, eto);
            },
            bloomCheck: this.Options.KeywordCheckOptions.BloomCheck,
            duplicateCheck: this.Options.KeywordCheckOptions.RedisCheck);
    }

    /// <summary>
    /// 二次重复性校验
    /// </summary>
    /// <returns></returns>
    protected override async Task<bool> DuplicateCheckAsync(string keyword, SpiderSourceFrom from)
    {
        string key = AppGlobalConstants.SPIDER_KEYWORDS_KEY;
        if (this.Options.KeywordCheckOptions.OnlyCurrentCategory)
        {
            key += $":{from.GetName()}";
        }

        bool result = await this.RedisService.SetAsync(key, keyword);
        return result;
    }

    /// <summary>
    /// 执行获取一级页面数据任务
    /// </summary>
    public async Task HandlePushEventAsync<T>(T eventData) where T : class, ISpiderPushEto
    {
        try
        {
            string targetUrl = string.Format(this.HomePage, eventData.Keyword);
            await this.WebElementLoadProvider.InvokeAsync(
                targetUrl,
                eventData.Keyword,
                drv => drv.FindElement(By.Id("rs_new")),
                async (root, keyword) =>
                {
                    if (root == null) return;

                    var resultContent = root.TryFindElements(By.TagName("a"));
                    if (resultContent is null or {Count: 0}) return;

                    ImmutableList<ChildPageDataItem> childPageDataItems = ImmutableList.Create<ChildPageDataItem>();
                    foreach (IWebElement element in resultContent)
                    {
                        string text = element.Text.Trim();
                        string href = element.GetAttribute("href");

                        if (this.Options.KeywordCheckOptions.IsEnableSimilarityCheck)
                        {
                            //执行相似度检测
                            double sim = StringHelper.Sim(eventData.Keyword, text);
                            if (sim * 100 < this.Options.KeywordCheckOptions.MinSimilarity)
                            {
                                continue;
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
                    }

                    if (childPageDataItems is {Count: > 0})
                    {
                        this.Logger.LogInformation("通道：{0}，关键字：{1}，一级页面：{2}条", eventData.SourceFrom.GetDescription(),
                            eventData.Keyword, childPageDataItems.Count.ToString());

                        var eto = eventData.SourceFrom.TryCreateEto(EtoType.Pull, eventData.SourceFrom,
                            eventData.Keyword, eventData.Keyword, childPageDataItems.ToList(), eventData.TraceCode,
                            eventData.IdentityId);

                        //保存采集到的标题
                        if (eto is ISpiderPullEto pullEto)
                        {
                            //此处不做消息队列发送，直接存储到数据库
                            await this.HandlePullEventAsync(pullEto);

                            List<SpiderContent_Keyword> list = pullEto.Items.Select(item =>
                                    new SpiderContent_Keyword(item.Title, pullEto.SourceFrom, eventData.TraceCode))
                                .ToList();
                            await this.SpiderKeywordRepository.InsertManyAsync(list);
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
    public async Task HandlePullEventAsync<T>(T eventData) where T : class, ISpiderPullEto
    {
        try
        {
            List<SpiderContent_Title> contents = new List<SpiderContent_Title>();
            foreach (var item in eventData.Items)
            {
                var content = new SpiderContent_Title(item.Title, item.Href, eventData.SourceFrom, eventData.TraceCode);
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