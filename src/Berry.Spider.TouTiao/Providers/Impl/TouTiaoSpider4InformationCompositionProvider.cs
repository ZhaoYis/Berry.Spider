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
using Volo.Abp.Guids;

namespace Berry.Spider.TouTiao;

/// <summary>
/// 今日头条：头条_资讯_作文板块
/// </summary>
[SpiderService(new[] {SpiderSourceFrom.TouTiao_Information_Composition})]
public class TouTiaoSpider4InformationCompositionProvider : ProviderBase<TouTiaoSpider4InformationCompositionProvider>,
    ISpiderProvider
{
    private IGuidGenerator GuidGenerator { get; }
    private IWebElementLoadProvider WebElementLoadProvider { get; }
    private IResolveJumpUrlProvider ResolveJumpUrlProvider { get; }
    private IRedisService RedisService { get; }
    private IEventBusPublisher DistributedEventBus { get; }
    private ISpiderContentCompositionRepository SpiderRepository { get; }
    private ISpiderContentKeywordRepository SpiderKeywordRepository { get; }
    private SpiderDomainService SpiderDomainService { get; }
    private SpiderOptions Options { get; }

    private string HomePage => "https://so.toutiao.com/search?keyword={0}&pd=information&dvpf=pc";

    public TouTiaoSpider4InformationCompositionProvider(ILogger<TouTiaoSpider4InformationCompositionProvider> logger,
        IGuidGenerator guidGenerator,
        IServiceProvider serviceProvider,
        IWebElementLoadProvider provider,
        IRedisService redisService,
        IEventBusPublisher eventBus,
        ISpiderContentCompositionRepository spiderRepository,
        ISpiderContentKeywordRepository keywordRepository,
        SpiderDomainService spiderDomainService,
        IOptionsSnapshot<SpiderOptions> options) : base(logger)
    {
        this.GuidGenerator = guidGenerator;
        this.WebElementLoadProvider = provider;
        this.ResolveJumpUrlProvider = serviceProvider.GetRequiredService<TouTiaoResolveJumpUrlProvider>();
        this.RedisService = redisService;
        this.DistributedEventBus = eventBus;
        this.SpiderRepository = spiderRepository;
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
        //验证一次
        bool result = await this.RedisService.SetAsync(GlobalConstants.SPIDER_KEYWORDS_KEY_PUSH, eventData.IdentityId);
        if (!result) return;

        string targetUrl = string.Format(this.HomePage, eventData.Keyword);
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
                    var a = element.TryFindElement(By.TagName("a"));
                    if (a != null)
                    {
                        string text = a.Text;
                        string href = a.GetAttribute("href");

                        //执行相似度检测
                        double sim = StringHelper.Sim(eventData.Keyword, text.Trim());
                        if (this.Options.KeywordCheckOptions.IsEnableSimilarityCheck)
                        {
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

    /// <summary>
    /// 执行根据一级页面采集到的地址获取二级页面具体目标数据任务
    /// </summary>
    /// <returns></returns>
    public async Task HandlePullEventAsync<T>(T eventData) where T : class, ISpiderPullEto
    {
        //验证一次
        bool result = await this.RedisService.SetAsync(GlobalConstants.SPIDER_KEYWORDS_KEY_PULL, eventData.IdentityId);
        if (!result) return;

        try
        {
            string groupId = this.GuidGenerator.Create().ToString("N");
            ImmutableList<SpiderContent_Composition> contentItems = ImmutableList.Create<SpiderContent_Composition>();

            foreach (var item in eventData.Items)
            {
                await this.WebElementLoadProvider.InvokeAsync(
                    item.Href,
                    drv => drv.FindElement(By.CssSelector(".article-content")),
                    async root =>
                    {
                        if (root == null) return;

                        var resultContent = root.TryFindElement(By.TagName("article"));
                        if (resultContent != null)
                        {
                            string content = resultContent.Text;
                            if (!string.IsNullOrEmpty(content))
                            {
                                SpiderContent_Composition spiderContent =
                                    new SpiderContent_Composition(item.Title, content, groupId, eventData.SourceFrom);
                                spiderContent.SetTraceCodeIfNotNull(eventData.TraceCode);
                                contentItems = contentItems.Add(spiderContent);
                            }
                        }

                        await Task.CompletedTask;
                    }
                );

                //修养生息20ms
                await Task.Delay(20);
            }

            //去重
            List<SpiderContent_Composition> todoSaveContentItems =
                contentItems.Where(c => !string.IsNullOrEmpty(c.Content)).ToList();
            await this.SpiderRepository.InsertManyAsync(todoSaveContentItems);
        }
        catch (Exception exception)
        {
            this.Logger.LogException(exception);
        }
    }
}