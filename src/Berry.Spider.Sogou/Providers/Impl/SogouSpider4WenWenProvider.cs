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

namespace Berry.Spider.Sogou;

/// <summary>
/// 搜狗：问问
/// </summary>
[SpiderService(new[] {SpiderSourceFrom.Sogou_WenWen})]
public class SogouSpider4WenWenProvider : ProviderBase<SogouSpider4WenWenProvider>, ISpiderProvider
{
    private IWebElementLoadProvider WebElementLoadProvider { get; }
    private ITextAnalysisProvider TextAnalysisProvider { get; }
    private IEventBusPublisher DistributedEventBus { get; }
    private IRedisService RedisService { get; }
    private ISpiderContentHighQualityQARepository SpiderRepository { get; }
    private ISpiderContentKeywordRepository SpiderKeywordRepository { get; }
    private SpiderDomainService SpiderDomainService { get; }
    private SpiderOptions Options { get; }

    private string HomePage => "https://wenwen.sogou.com";

    public SogouSpider4WenWenProvider(ILogger<SogouSpider4WenWenProvider> logger,
        IWebElementLoadProvider provider,
        IServiceProvider serviceProvider,
        IEventBusPublisher eventBus,
        IRedisService redisService,
        ISpiderContentHighQualityQARepository repository,
        ISpiderContentKeywordRepository keywordRepository,
        SpiderDomainService spiderDomainService,
        IOptionsSnapshot<SpiderOptions> options) : base(logger)
    {
        this.WebElementLoadProvider = provider;
        this.TextAnalysisProvider = serviceProvider.GetRequiredService<SogouSpider4WenWenTextAnalysisProvider>();
        this.DistributedEventBus = eventBus;
        this.RedisService = redisService;
        this.SpiderRepository = repository;
        this.SpiderKeywordRepository = keywordRepository;
        this.SpiderDomainService = spiderDomainService;
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

            //获取url地址
            string realUrl = await this.WebElementLoadProvider.AutoClickAsync(this.HomePage, eventData.Keyword,
                By.Id("sb"),
                By.Id("searchBtn"));
            if (string.IsNullOrWhiteSpace(realUrl)) return;

            await this.WebElementLoadProvider.InvokeAsync(
                realUrl,
                drv => drv.FindElement(By.CssSelector(".results")),
                async root =>
                {
                    if (root == null) return;

                    var resultContent = root.TryFindElements(By.TagName("a"));
                    if (resultContent is null or {Count: 0}) return;

                    ImmutableList<ChildPageDataItem> childPageDataItems = ImmutableList.Create<ChildPageDataItem>();
                    foreach (IWebElement element in resultContent)
                    {
                        string text = element.Text;
                        string href = element.GetAttribute("href");

                        //执行相似度检测
                        double sim = StringHelper.Sim(eventData.Keyword, text.Trim());
                        if (this.Options.KeywordCheckOptions.IsEnableSimilarityCheck)
                        {
                            if (sim * 100 < this.Options.KeywordCheckOptions.MinSimilarity)
                            {
                                return;
                            }
                        }

                        childPageDataItems = childPageDataItems.Add(new ChildPageDataItem
                        {
                            Title = text,
                            Href = href
                        });
                    }

                    if (childPageDataItems is {Count: > 0})
                    {
                        this.Logger.LogInformation("通道：{0}，关键字：{1}，一级页面：{2}条", eventData.SourceFrom.GetDescription(),
                            eventData.Keyword, childPageDataItems.Count);

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
                    drv => drv.FindElement(By.Id("bestAnswers")),
                    async root =>
                    {
                        if (root == null) return;

                        var resultContent = root.TryFindElements(By.CssSelector(".replay-info"));
                        if (resultContent is null or {Count: 0}) return;

                        foreach (IWebElement element in resultContent)
                        {
                            var answerList = element.TryFindElements(By.TagName("pre"));
                            if (answerList is null or {Count: 0}) continue;

                            var realAnswerList = answerList
                                .Where(c => c.GetAttribute("class").Contains("answer_con"))
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
                SpiderContent_HighQualityQA spiderContent =
                    await this.SpiderDomainService.BuildHighQualityContentAsync(eventData.Title, eventData.SourceFrom,
                        contentItems, traceCode: eventData.TraceCode);
                await this.SpiderRepository.InsertAsync(spiderContent);
                this.Logger.LogInformation("落库成功关键字：{0}，标题：{0}，共计：{1}行记录", eventData.Keyword, spiderContent.Title,
                    contentItems.Count);
            }
        }
        catch (Exception exception)
        {
            this.Logger.LogException(exception);
        }
    }
}