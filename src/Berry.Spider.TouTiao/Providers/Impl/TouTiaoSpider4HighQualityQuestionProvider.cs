using System.Collections.Concurrent;
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
using Volo.Abp.Timing;

namespace Berry.Spider.TouTiao;

/// <summary>
/// 今日头条：优质_问答
/// </summary>
[SpiderService(new[] {SpiderSourceFrom.TouTiao_HighQuality_Question, SpiderSourceFrom.TouTiao_HighQuality_Question_Ext_NO_1})]
public class TouTiaoSpider4HighQualityQuestionProvider : ProviderBase<TouTiaoSpider4HighQualityQuestionProvider>, ISpiderProvider
{
    private IWebElementLoadProvider WebElementLoadProvider { get; }
    private IResolveJumpUrlProvider ResolveJumpUrlProvider { get; }
    private ISpiderContentHighQualityQARepository SpiderRepository { get; }
    private ISpiderContentKeywordRepository SpiderKeywordRepository { get; }
    private SpiderDomainService SpiderDomainService { get; }
    private IEventBusPublisher DistributedEventBus { get; }
    private IRedisService RedisService { get; }
    private SpiderOptions Options { get; }
    private IClock Clock { get; }

    private string HomePage => "https://so.toutiao.com/search?wid_ct={0}&keyword={1}&pd=question&dvpf=pc&page_num=0&source=input";

    public TouTiaoSpider4HighQualityQuestionProvider(ILogger<TouTiaoSpider4HighQualityQuestionProvider> logger,
        IWebElementLoadProvider provider,
        IServiceProvider serviceProvider,
        SpiderDomainService spiderDomainService,
        ISpiderContentHighQualityQARepository repository,
        ISpiderContentKeywordRepository keywordRepository,
        IEventBusPublisher eventBus,
        IRedisService redisService,
        IOptionsSnapshot<SpiderOptions> options,
        IClock clock) : base(logger)
    {
        this.WebElementLoadProvider = provider;
        this.ResolveJumpUrlProvider = serviceProvider.GetRequiredService<TouTiaoResolveJumpUrlProvider>();
        this.SpiderRepository = repository;
        this.SpiderKeywordRepository = keywordRepository;
        this.SpiderDomainService = spiderDomainService;
        this.DistributedEventBus = eventBus;
        this.RedisService = redisService;
        this.Options = options.Value;
        this.Clock = clock;
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
        string key = GlobalConstants.SPIDER_KEYWORDS_KEY;
        if (this.Options.KeywordCheckOptions.OnlyCurrentCategory)
        {
            key += $":{from.ToString()}";
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
            //关键字采集唯一性验证
            if (this.Options.IsEnablePushUniqVerif)
            {
                bool result = await this.RedisService.SetAsync(GlobalConstants.SPIDER_KEYWORDS_KEY_PUSH, eventData.IdentityId);
                if (!result) return;
            }

            string targetUrl = string.Format(this.HomePage, this.Clock.Now.ToTimestamp(), eventData.Keyword);
            await this.WebElementLoadProvider.InvokeAsync(
                targetUrl,
                drv => drv.FindElement(By.CssSelector(".s-result-list")),
                async root =>
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
                            string text = a.Text;
                            string href = a.GetAttribute("href");

                            if (this.Options.KeywordCheckOptions.IsEnableSimilarityCheck)
                            {
                                //执行相似度检测
                                double sim = StringHelper.Sim(eventData.Keyword, text.Trim());
                                if (sim * 100 < this.Options.KeywordCheckOptions.MinSimilarity)
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
                        }
                    }

                    if (childPageDataItems is {Count: > 0})
                    {
                        this.Logger.LogInformation("通道：{0}，关键字：{1}，一级页面：{2}条", eventData.SourceFrom.GetDescription(), eventData.Keyword, childPageDataItems.Count);

                        var eto = eventData.SourceFrom.TryCreateEto(EtoType.Pull, eventData.SourceFrom,
                            eventData.Keyword, eventData.Keyword, childPageDataItems.ToList(), eventData.TraceCode,
                            eventData.IdentityId);
                        await this.DistributedEventBus.PublishAsync(eto.TryGetRoutingKey(), eto);

                        //保存采集到的标题
                        if (eto is ISpiderPullEto pullEto)
                        {
                            List<SpiderContent_Keyword> list = pullEto.Items.Select(item => new SpiderContent_Keyword(item.Title, pullEto.SourceFrom, eventData.TraceCode)).ToList();
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
            //关键字采集唯一性验证
            if (this.Options.IsEnablePullUniqVerif)
            {
                bool result = await this.RedisService.SetAsync(GlobalConstants.SPIDER_KEYWORDS_KEY_PULL, eventData.IdentityId);
                if (!result) return;
            }

            ConcurrentDictionary<string, List<string>> contentItems = new ConcurrentDictionary<string, List<string>>();
            foreach (var item in eventData.Items)
            {
                await this.WebElementLoadProvider.InvokeAsync(
                    item.Href,
                    drv => drv.FindElement(By.CssSelector(".s-container")),
                    async root =>
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

                            ImmutableList<string> answerContentItems = ImmutableList.Create<string>();
                            foreach (IWebElement answer in realAnswerList)
                            {
                                if (!string.IsNullOrWhiteSpace(answer.Text))
                                {
                                    if (this.Options.HighQualityAnswerOptions.IsEnable)
                                    {
                                        //TODO：后续优化为计算真实字符数（中文、英文、符号、表情等混合时）
                                        if (answer.Text.Length.Between(
                                                this.Options.HighQualityAnswerOptions.MinLength,
                                                this.Options.HighQualityAnswerOptions.MaxLength))
                                        {
                                            answerContentItems = answerContentItems.Add(answer.Text);
                                        }
                                    }
                                    else
                                    {
                                        answerContentItems = answerContentItems.Add(answer.Text);
                                    }
                                }
                            }

                            //去重
                            List<string> todoSaveContentItems = answerContentItems
                                .Where(c => !string.IsNullOrEmpty(c)).Distinct().ToList();
                            contentItems.TryAdd(item.Title, todoSaveContentItems);
                        }

                        await Task.CompletedTask;
                    }
                );

                //修养生息20ms
                await Task.Delay(20);
            }

            //检查落库最小记录数
            if (contentItems.Count > this.Options.HighQualityAnswerOptions.MinSaveRecordCount)
            {
                SpiderContent_HighQualityQA spiderContent = await this.SpiderDomainService.BuildHighQualityContentAsync(eventData.Title, eventData.SourceFrom, contentItems, traceCode: eventData.TraceCode, identityId: eventData.IdentityId);
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