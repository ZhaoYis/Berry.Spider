using Berry.Spider.Abstractions;
using Berry.Spider.Contracts;
using Berry.Spider.Core;
using Berry.Spider.Domain;
using Berry.Spider.EventBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenQA.Selenium;
using Berry.Spider.FreeRedis;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Timing;

namespace Berry.Spider.TouTiao;

/// <summary>
/// 今日头条：问答
/// </summary>
[Spider(SpiderSourceFrom.TouTiao_Question)]
public class TouTiaoSpider4QuestionProvider : ProviderBase<TouTiaoSpider4QuestionProvider>, ISpiderProvider
{
    private IWebElementLoadProvider WebElementLoadProvider { get; }
    private ITextAnalysisProvider TextAnalysisProvider { get; }
    private IResolveJumpUrlProvider ResolveJumpUrlProvider { get; }
    private ISpiderContentRepository SpiderRepository { get; }
    private SpiderDomainService SpiderDomainService { get; }
    private IClock Clock { get; }
    private IEventBusPublisher EventBusPublisher { get; }
    private IRedisService RedisService { get; }
    private IOptionsSnapshot<SpiderOptions> Options { get; }

    private string HomePage => "https://so.toutiao.com/search?keyword={0}&pd=question&dvpf=pc";

    public TouTiaoSpider4QuestionProvider(ILogger<TouTiaoSpider4QuestionProvider> logger,
        IWebElementLoadProvider provider,
        IServiceProvider serviceProvider,
        SpiderDomainService spiderDomainService,
        ISpiderContentRepository repository,
        IClock clock,
        IEventBusPublisher eventBus,
        IRedisService redisService,
        IOptionsSnapshot<SpiderOptions> options) : base(logger)
    {
        this.WebElementLoadProvider = provider;
        this.TextAnalysisProvider = serviceProvider.GetRequiredService<TouTiaoQuestionTextAnalysisProvider>();
        this.ResolveJumpUrlProvider = serviceProvider.GetRequiredService<TouTiaoResolveJumpUrlProvider>();
        this.SpiderRepository = repository;
        this.SpiderDomainService = spiderDomainService;
        this.Clock = clock;
        this.EventBusPublisher = eventBus;
        this.RedisService = redisService;
        this.Options = options;
    }

    /// <summary>
    /// 向队列推送源数据
    /// </summary>
    /// <returns></returns>
    public async Task PushAsync<T>(T push) where T : class, ISpiderPushEto
    {
        await this.CheckAsync(push.Keyword,
            checkSuccessCallback: async () => { await this.EventBusPublisher.PublishAsync(push.TryGetEventName(), push); },
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
            key += $":{SpiderSourceFrom.TouTiao_Question}";
        }

        bool result = await this.RedisService.SetAsync(key, keyword);
        return result;
    }

    /// <summary>
    /// 执行获取一级页面数据任务
    /// </summary>
    /// <returns></returns>
    public async Task ExecuteAsync<T>(T request) where T : class, ISpiderRequest
    {
        string targetUrl = string.Format(this.HomePage, request.Keyword);
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

                    var eto = new TouTiaoSpider4QuestionPullEto
                    {
                        Keyword = request.Keyword,
                        Title = request.Keyword
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
                        await this.EventBusPublisher.PublishAsync(eto.TryGetEventName(), eto);
                        this.Logger.LogInformation("事件发布成功，等待消费...");
                    }
                }
            });
    }

    /// <summary>
    /// 执行根据一级页面采集到的地址获取二级页面具体目标数据任务
    /// </summary>
    /// <returns></returns>
    public async Task HandleEventAsync<T>(T eventData) where T : class, ISpiderPullEto
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
                            }, async (element, token) =>
                            {
                                var answerList = element.TryFindElements(By.TagName("div"));
                                if (answerList is { Count: > 0 })
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
                                                    contentItems.AddRange(list);
                                                    this.Logger.LogInformation("总共获取到记录：" + list.Count);
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
            contentItems = contentItems.Distinct().ToList();
            SpiderContent? spiderContent =
                await this.SpiderDomainService.BuildContentAsync(eventData.Title, eventData.SourceFrom, contentItems);
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