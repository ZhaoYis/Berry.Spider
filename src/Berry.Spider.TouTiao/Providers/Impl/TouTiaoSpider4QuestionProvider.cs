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

namespace Berry.Spider.TouTiao;

/// <summary>
/// 今日头条：问答
/// </summary>
[SpiderService(new[] {SpiderSourceFrom.TouTiao_Question, SpiderSourceFrom.TouTiao_Question_Ext_NO_1})]
public class TouTiaoSpider4QuestionProvider : ProviderBase<TouTiaoSpider4QuestionProvider>, ISpiderProvider
{
    private IWebElementLoadProvider WebElementLoadProvider { get; }
    private ITextAnalysisProvider TextAnalysisProvider { get; }
    private IResolveJumpUrlProvider ResolveJumpUrlProvider { get; }
    private ISpiderContentRepository SpiderRepository { get; }
    private ISpiderContentKeywordRepository SpiderKeywordRepository { get; }
    private SpiderDomainService SpiderDomainService { get; }
    private IEventBusPublisher DistributedEventBus { get; }
    private IRedisService RedisService { get; }
    private SpiderOptions Options { get; }

    private string HomePage => "https://so.toutiao.com/search?keyword={0}&pd=question&dvpf=pc";

    public TouTiaoSpider4QuestionProvider(ILogger<TouTiaoSpider4QuestionProvider> logger,
        IWebElementLoadProvider provider,
        IServiceProvider serviceProvider,
        SpiderDomainService spiderDomainService,
        ISpiderContentRepository repository,
        ISpiderContentKeywordRepository keywordRepository,
        IEventBusPublisher eventBus,
        IRedisService redisService,
        IOptionsSnapshot<SpiderOptions> options) : base(logger)
    {
        this.WebElementLoadProvider = provider;
        this.TextAnalysisProvider = serviceProvider.GetRequiredService<TouTiaoQuestionTextAnalysisProvider>();
        this.ResolveJumpUrlProvider = serviceProvider.GetRequiredService<TouTiaoResolveJumpUrlProvider>();
        this.SpiderRepository = repository;
        this.SpiderKeywordRepository = keywordRepository;
        this.SpiderDomainService = spiderDomainService;
        this.DistributedEventBus = eventBus;
        this.RedisService = redisService;
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
    /// <returns></returns>
    public async Task HandlePushEventAsync<T>(T eventData) where T : class, ISpiderPushEto
    {
        try
        {
            string targetUrl = string.Format(this.HomePage, eventData.Keyword);
            await this.WebElementLoadProvider.InvokeAsync(
                targetUrl,
                eventData.Keyword,
                drv => drv.FindElement(By.CssSelector(".s-result-list")),
                async (root, keyword) =>
                {
                    if (root == null) return;

                    var resultContent = root.TryFindElements(By.CssSelector(".result-content"));
                    if (resultContent is null or {Count: 0}) return;

                    ImmutableList<ChildPageDataItem> childPageDataItems = ImmutableList.Create<ChildPageDataItem>();
                    foreach (IWebElement element in resultContent)
                    {
                        //TODO:只取 大家都在问 的部分

                        var a = element.TryFindElement(By.TagName("a"));
                        if (a != null)
                        {
                            string text = a.Text.Trim();
                            string href = a.GetAttribute("href");

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
                    }

                    if (childPageDataItems is {Count: > 0})
                    {
                        this.Logger.LogInformation("通道：{0}，关键字：{1}，一级页面：{2}条", eventData.SourceFrom.GetDescription(),
                            eventData.Keyword, childPageDataItems.Count.ToString());

                        var eto = eventData.SourceFrom.TryCreateEto(EtoType.Pull, eventData.SourceFrom,
                            eventData.Keyword, eventData.Keyword, childPageDataItems.ToList(), eventData.TraceCode,
                            eventData.IdentityId);
                        await this.DistributedEventBus.PublishAsync(eto.TryGetRoutingKey(), eto);

                        //保存采集到的标题
                        if (eto is ISpiderPullEto pullEto)
                        {
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
    /// <returns></returns>
    public async Task HandlePullEventAsync<T>(T eventData) where T : class, ISpiderPullEto
    {
        try
        {
            ImmutableList<string> contentItems = ImmutableList.Create<string>();
            await this.WebElementLoadProvider.BatchInvokeAsync(
                eventData.Items.ToDictionary(k => k.Title, v => v.Href),
                drv => drv.FindElement(By.CssSelector(".s-container")),
                async (root, keyword) =>
                {
                    if (root == null) return;

                    var resultContent = root.TryFindElements(By.CssSelector(".list"));
                    if (resultContent is null or {Count: 0}) return;

                    foreach (IWebElement element in resultContent)
                    {
                        var answerList = element.TryFindElements(By.TagName("div"));
                        if (answerList is null or {Count: 0}) continue;

                        var realAnswerList = answerList
                            .Where(c => c.GetAttribute("class").StartsWith("answer_layout_wrapper_"))
                            .ToList();
                        if (realAnswerList is null or {Count: 0}) continue;

                        foreach (IWebElement answer in realAnswerList)
                        {
                            if (!string.IsNullOrWhiteSpace(answer.Text))
                            {
                                //解析内容
                                var list = await this.TextAnalysisProvider.InvokeAsync(answer.Text);
                                if (list.Count > 0)
                                {
                                    contentItems = contentItems.AddRange(list);
                                    this.Logger.LogInformation("总共解析到记录：{0}", list.Count);
                                }
                            }
                        }
                    }
                }
            );

            //去重
            List<string> todoSaveContentItems = contentItems.Where(c => !string.IsNullOrEmpty(c)).Distinct().ToList();
            SpiderContent? spiderContent = await this.SpiderDomainService.BuildContentAsync(eventData.Title,
                eventData.SourceFrom, todoSaveContentItems, traceCode: eventData.TraceCode, identityId: eventData.IdentityId);
            if (spiderContent != null)
            {
                await this.SpiderRepository.InsertAsync(spiderContent);
                this.Logger.LogInformation("落库成功关键字：{0}，标题：{0}，共计：{1}行记录", eventData.Keyword, spiderContent.Title, contentItems.Count);
            }
        }
        catch (Exception exception)
        {
            this.Logger.LogException(exception);
        }
    }
}