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
    private IOptionsSnapshot<SpiderOptions> Options { get; }

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
    /// <returns></returns>
    public async Task HandlePushEventAsync<T>(T eventData) where T : class, ISpiderPushEto
    {
        string targetUrl = string.Format(this.HomePage, eventData.Keyword);
        await this.WebElementLoadProvider.InvokeAsync(
            targetUrl,
            drv => drv.FindElement(By.ClassName("s-result-list")),
            async root =>
            {
                if (root == null) return;

                var resultContent = root.TryFindElements(By.ClassName("result-content"));
                if (resultContent is {Count: > 0})
                {
                    this.Logger.LogInformation("总共采集到记录：" + resultContent.Count);

                    ImmutableList<ChildPageDataItem> childPageDataItems = ImmutableList.Create<ChildPageDataItem>();
                    await Parallel.ForEachAsync(resultContent, new ParallelOptions
                    {
                        MaxDegreeOfParallelism = GlobalConstants.ParallelMaxDegreeOfParallelism
                    }, async (element, token) =>
                    {
                        //TODO:只取 大家都在问 的部分

                        var a = element.TryFindElement(By.TagName("a"));
                        if (a != null)
                        {
                            string text = a.Text;
                            string href = a.GetAttribute("href");

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
                        }
                    });

                    if (childPageDataItems.Any())
                    {
                        var eto = eventData.SourceFrom.TryCreateEto(EtoType.Pull, eventData.SourceFrom, eventData.Keyword, eventData.Keyword, childPageDataItems.ToList());
                        await this.DistributedEventBus.PublishAsync(eto.TryGetRoutingKey(), eto);

                        //保存采集到的标题
                        if (eto is ISpiderPullEto pullEto)
                        {
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
    /// <returns></returns>
    public async Task HandlePullEventAsync<T>(T eventData) where T : class, ISpiderPullEto
    {
        try
        {
            ImmutableList<string> contentItems = ImmutableList.Create<string>();
            foreach (var item in eventData.Items)
            {
                await this.WebElementLoadProvider.InvokeAsync(
                    item.Href,
                    drv => drv.FindElement(By.ClassName("s-container")),
                    async root =>
                    {
                        if (root == null) return;

                        var resultContent = root.TryFindElements(By.ClassName("list"));
                        if (resultContent is {Count: > 0})
                        {
                            await Parallel.ForEachAsync(resultContent, new ParallelOptions
                            {
                                MaxDegreeOfParallelism = GlobalConstants.ParallelMaxDegreeOfParallelism
                            }, async (element, token) =>
                            {
                                var answerList = element.TryFindElements(By.TagName("div"));
                                if (answerList is {Count: > 0})
                                {
                                    var realAnswerList = answerList
                                        .Where(c => c.GetAttribute("class").StartsWith("answer_layout_wrapper_"))
                                        .ToList();

                                    if (realAnswerList.Any())
                                    {
                                        await Parallel.ForEachAsync(realAnswerList, new ParallelOptions
                                        {
                                            MaxDegreeOfParallelism = GlobalConstants.ParallelMaxDegreeOfParallelism
                                        }, async (answer, cancellationToken) =>
                                        {
                                            if (!string.IsNullOrWhiteSpace(answer.Text))
                                            {
                                                //解析内容
                                                var list = await this.TextAnalysisProvider.InvokeAsync(answer.Text);
                                                if (list.Count > 0)
                                                {
                                                    contentItems = contentItems.AddRange(list);
                                                    this.Logger.LogInformation("总共解析到记录：" + list.Count);
                                                }
                                            }
                                        });
                                    }
                                }
                            });
                        }
                    }
                );
            }

            //去重
            List<string> todoSaveContentItems = contentItems.Where(c => !string.IsNullOrEmpty(c)).Distinct().ToList();
            SpiderContent? spiderContent = await this.SpiderDomainService.BuildContentAsync(eventData.Title, eventData.SourceFrom, todoSaveContentItems);
            if (spiderContent != null)
            {
                await this.SpiderRepository.InsertAsync(spiderContent);
                this.Logger.LogInformation("落库成功，标题：" + spiderContent.Title + "，共计：" + contentItems.Count + "条记录");
            }
        }
        catch (Exception exception)
        {
            this.Logger.LogException(exception);
        }
    }
}