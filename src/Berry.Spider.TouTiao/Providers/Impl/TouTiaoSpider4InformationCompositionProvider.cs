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
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Guids;

namespace Berry.Spider.TouTiao;

/// <summary>
/// 今日头条：头条_资讯_作文板块
/// </summary>
[Spider(SpiderSourceFrom.TouTiao_Information_Composition)]
public class TouTiaoSpider4InformationCompositionProvider : ProviderBase<TouTiaoSpider4InformationCompositionProvider>,
    ISpiderProvider
{
    private IGuidGenerator GuidGenerator { get; }
    private IWebElementLoadProvider WebElementLoadProvider { get; }
    private IResolveJumpUrlProvider ResolveJumpUrlProvider { get; }
    private IRedisService RedisService { get; }
    private IEventBusPublisher DistributedEventBus { get; }
    private ISpiderContentRepository SpiderRepository { get; }
    private SpiderDomainService SpiderDomainService { get; }
    private IOptionsSnapshot<SpiderOptions> Options { get; }

    private string HomePage => "https://so.toutiao.com/search?keyword={0}&pd=information&dvpf=pc";

    public TouTiaoSpider4InformationCompositionProvider(ILogger<TouTiaoSpider4InformationCompositionProvider> logger,
        IGuidGenerator guidGenerator,
        IServiceProvider serviceProvider,
        IWebElementLoadProvider provider,
        IRedisService redisService,
        IEventBusPublisher eventBus,
        ISpiderContentRepository spiderRepository,
        SpiderDomainService spiderDomainService,
        IOptionsSnapshot<SpiderOptions> options) : base(logger)
    {
        this.GuidGenerator = guidGenerator;
        this.WebElementLoadProvider = provider;
        this.ResolveJumpUrlProvider = serviceProvider.GetRequiredService<TouTiaoResolveJumpUrlProvider>();
        this.RedisService = redisService;
        this.DistributedEventBus = eventBus;
        this.SpiderRepository = spiderRepository;
        this.SpiderDomainService = spiderDomainService;
        this.Options = options;
    }

    /// <summary>
    /// 向队列推送源数据
    /// </summary>
    /// <returns></returns>
    public async Task PushAsync<T>(T push) where T : class, ISpiderPushEto
    {
        await this.CheckAsync(push.Keyword, async () => { await this.DistributedEventBus.PublishAsync(push.TryGetEventName(), push); },
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
            key += $":{SpiderSourceFrom.TouTiao_Information_Composition}";
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

                    var eto = new TouTiaoSpider4InformationCompositionPullEto
                    {
                        Keyword = request.Keyword,
                        Title = request.Keyword
                    };

                    await Parallel.ForEachAsync(resultContent, new ParallelOptions
                    {
                        MaxDegreeOfParallelism = GlobalConstants.ParallelMaxDegreeOfParallelism
                    }, async (element, token) =>
                    {
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
                        await this.DistributedEventBus.PublishAsync(eto.TryGetEventName(), eto);
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
            string groupId = this.GuidGenerator.Create().ToString("N");
            List<SpiderContent> contentItems = new List<SpiderContent>();

            foreach (var item in eventData.Items)
            {
                await this.WebElementLoadProvider.InvokeAsync(
                    item.Href,
                    drv => drv.FindElement(By.ClassName("article-content")),
                    async root =>
                    {
                        if (root == null) return;

                        var resultContent = root.TryFindElement(By.TagName("article"));
                        if (resultContent != null)
                        {
                            string content = resultContent.Text;
                            if (!string.IsNullOrEmpty(content))
                            {
                                SpiderContent spiderContent =
                                    new SpiderContent(item.Title, content, groupId, eventData.SourceFrom);
                                contentItems.Add(spiderContent);
                            }
                        }

                        await Task.Delay(1);
                    }
                );
            }

            await this.SpiderRepository.InsertManyAsync(contentItems);
        }
        catch (Exception exception)
        {
            this.Logger.LogException(exception);
        }
    }
}