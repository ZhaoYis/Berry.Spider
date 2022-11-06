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
/// 今日头条：优质_问答
/// </summary>
[Spider(SpiderSourceFrom.TouTiao_HighQuality_Question)]
public class TouTiaoSpider4HighQualityQuestionProvider : ProviderBase<TouTiaoSpider4HighQualityQuestionProvider>, ISpiderProvider
{
    private IWebElementLoadProvider WebElementLoadProvider { get; }
    private IResolveJumpUrlProvider ResolveJumpUrlProvider { get; }
    private ISpiderContentRepository SpiderRepository { get; }
    private SpiderDomainService SpiderDomainService { get; }
    private IEventBusPublisher DistributedEventBus { get; }
    private IRedisService RedisService { get; }
    private IOptionsSnapshot<SpiderOptions> Options { get; }

    private string HomePage => "https://so.toutiao.com/search?keyword={0}&pd=question&dvpf=pc";

    public TouTiaoSpider4HighQualityQuestionProvider(ILogger<TouTiaoSpider4HighQualityQuestionProvider> logger,
        IWebElementLoadProvider provider,
        IServiceProvider serviceProvider,
        SpiderDomainService spiderDomainService,
        ISpiderContentRepository repository,
        IEventBusPublisher eventBus,
        IRedisService redisService,
        IOptionsSnapshot<SpiderOptions> options) : base(logger)
    {
        this.WebElementLoadProvider = provider;
        this.ResolveJumpUrlProvider = serviceProvider.GetRequiredService<TouTiaoResolveJumpUrlProvider>();
        this.SpiderRepository = repository;
        this.SpiderDomainService = spiderDomainService;
        this.DistributedEventBus = eventBus;
        this.RedisService = redisService;
        this.Options = options;
    }

    /// <summary>
    /// 向队列推送源数据
    /// </summary>
    /// <returns></returns>
    public async Task PushAsync<T>(T push) where T : class, ISpiderPushEto
    {
        await this.DistributedEventBus.PublishAsync(push.TryGetRoutingKey(), push);
        // await this.CheckAsync(push.Keyword,
        //     checkSuccessCallback: async () => { await this.DistributedEventBus.PublishAsync(push.TryGetRoutingKey(), push); },
        //     bloomCheck: this.Options.Value.KeywordCheckOptions.BloomCheck,
        //     duplicateCheck: this.Options.Value.KeywordCheckOptions.RedisCheck);
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
            key += $":{SpiderSourceFrom.TouTiao_HighQuality_Question.ToString()}";
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
                if (resultContent is { Count: > 0 })
                {
                    this.Logger.LogInformation("总共获取到记录：" + resultContent.Count);

                    var eto = new TouTiaoSpider4HighQualityQuestionPullEto
                    {
                        Keyword = eventData.Keyword,
                        Title = eventData.Keyword
                    };

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

                            string realHref = await this.ResolveJumpUrlProvider.ResolveAsync(href);
                            if (!string.IsNullOrEmpty(realHref))
                            {
                                eto.Items.Add(new ChildPageDataItem
                                {
                                    Title = text,
                                    Href = realHref
                                });

                                this.Logger.LogInformation(text + "  ---> " + href);
                            }
                        }
                    });

                    if (eto.Items.Any())
                    {
                        await this.DistributedEventBus.PublishAsync(eto.TryGetRoutingKey(), eto);
                        this.Logger.LogInformation("事件发布成功，等待消费...");
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
            List<string> contentItems = new List<string>();
            foreach (var item in eventData.Items)
            {
                await this.WebElementLoadProvider.InvokeAsync(
                    item.Href,
                    drv => drv.FindElement(By.ClassName("s-container")),
                    async root =>
                    {
                        if (root == null) return;

                        var resultContent = root.TryFindElements(By.ClassName("list"));
                        if (resultContent is { Count: > 0 })
                        {
                            await Parallel.ForEachAsync(resultContent, new ParallelOptions
                            {
                                MaxDegreeOfParallelism = GlobalConstants.ParallelMaxDegreeOfParallelism
                            }, (element, token) =>
                            {
                                var answerList = element.TryFindElements(By.TagName("div"));
                                if (answerList is { Count: > 0 })
                                {
                                    var realAnswerList = answerList
                                        .Where(c => c.GetAttribute("class").StartsWith("answer_layout_wrapper_"))
                                        .ToList();

                                    if (realAnswerList.Any())
                                    {
                                        //TODO：后续可以做成解析器

                                        //获取回答最长的那一个答案
                                        string? maxLengthAnswer = realAnswerList.MaxBy(a => a.Text.Length)?.Text;
                                        if (!string.IsNullOrEmpty(maxLengthAnswer))
                                        {
                                            contentItems.Add(maxLengthAnswer);
                                        }
                                    }
                                }

                                return ValueTask.CompletedTask;
                            });
                        }
                    }
                );
            }

            SpiderContent? spiderContent =
                await this.SpiderDomainService.BuildHighQualityContentAsync(eventData.Title, eventData.SourceFrom, contentItems);
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