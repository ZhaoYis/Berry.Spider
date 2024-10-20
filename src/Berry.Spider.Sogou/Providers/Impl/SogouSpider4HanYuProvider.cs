using Berry.Spider.Application.Contracts;
using Berry.Spider.Core;
using Berry.Spider.Domain;
using Berry.Spider.EventBus;
using Berry.Spider.FreeRedis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenQA.Selenium;

namespace Berry.Spider.Sogou;

/// <summary>
/// 搜狗：汉语
/// </summary>
[SpiderService(new[] { SpiderSourceFrom.Sogou_HanYu })]
public class SogouSpider4HanYuProvider : ProviderBase<SogouSpider4HanYuProvider>, ISpiderProvider
{
    private IWebElementLoadProvider WebElementLoadProvider { get; }
    private IEventBusPublisher DistributedEventBus { get; }
    private IRedisService RedisService { get; }
    private SpiderDomainService SpiderDomainService { get; }
    private ISpiderContentRepository SpiderRepository { get; }
    private SpiderOptions Options { get; }

    private string HomePage => "https://hanyu.sogou.com";

    public SogouSpider4HanYuProvider(IOptionsSnapshot<SpiderOptions> options,
        IWebElementLoadProvider webElementLoadProvider,
        IEventBusPublisher eventBus,
        IRedisService redisService,
        SpiderDomainService spiderDomainService,
        ISpiderContentRepository spiderRepository,
        ILogger<SogouSpider4HanYuProvider> logger) : base(logger)
    {
        this.Options = options.Value;
        this.WebElementLoadProvider = webElementLoadProvider;
        this.RedisService = redisService;
        this.SpiderDomainService = spiderDomainService;
        this.SpiderRepository = spiderRepository;
        this.DistributedEventBus = eventBus;
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
            await this.WebElementLoadProvider.AutoClickAndInvokeAsync(
                this.HomePage,
                eventData.Keyword,
                By.Name("query"),
                By.XPath(@"//*[@id='pageApp']/div/div[3]/span[2]/input"),
                drv => drv.FindElement(By.CssSelector(".results")),
                async (root, keyword) =>
                {
                    if (root == null) return;

                    //清除tab-div里面的内容
                    IWebElement tabElement = root.FindElement(By.CssSelector(".tab-hanyu"));
                    tabElement?.Clear();

                    var title = keyword.ToString() ?? string.Empty;
                    var resultContent = root.Text;
                    SpiderContent spiderContent = await this.SpiderDomainService.BuildContentAsync(title,
                        eventData.SourceFrom, resultContent, traceCode: eventData.TraceCode, identityId: eventData.IdentityId);
                    await this.SpiderRepository.InsertAsync(spiderContent);
                    this.Logger.LogInformation("落库成功关键字：{Keyword}，标题：{Title}", eventData.Keyword, spiderContent.Title);
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
    public Task HandlePullEventAsync<T>(T eventData) where T : class, ISpiderPullEto
    {
        return Task.CompletedTask;
    }
}