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
/// 今日头条：资讯
/// </summary>
[SpiderService(new[] { SpiderSourceFrom.TouTiao_Information })]
public class TouTiaoSpider4InformationProvider : ProviderBase<TouTiaoSpider4InformationProvider>, ISpiderProvider
{
    private IWebElementLoadProvider WebElementLoadProvider { get; }
    private IResolveJumpUrlProvider ResolveJumpUrlProvider { get; }
    private ISpiderContentKeywordRepository SpiderKeywordRepository { get; }
    private IRedisService RedisService { get; }
    private IEventBusPublisher DistributedEventBus { get; }
    private SpiderOptions Options { get; }

    private string HomePage => "https://so.toutiao.com/search?keyword={0}&pd=information&dvpf=pc";

    public TouTiaoSpider4InformationProvider(ILogger<TouTiaoSpider4InformationProvider> logger,
        IWebElementLoadProvider provider,
        IServiceProvider serviceProvider,
        ISpiderContentKeywordRepository keywordRepository,
        IRedisService redisService,
        IEventBusPublisher eventBus,
        IOptionsSnapshot<SpiderOptions> options) : base(logger)
    {
        this.WebElementLoadProvider = provider;
        this.ResolveJumpUrlProvider = serviceProvider.GetRequiredService<TouTiaoResolveJumpUrlProvider>();
        this.RedisService = redisService;
        this.DistributedEventBus = eventBus;
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
            string targetUrl = string.Format(this.HomePage, eventData.Keyword);
            await this.WebElementLoadProvider.InvokeAsync(
                targetUrl,
                eventData.Keyword,
                drv => drv.FindElement(By.CssSelector(".s-result-list")),
                async (root, keyword) =>
                {
                    if (root == null) return;

                    var resultContent = root.TryFindElements(By.CssSelector(".result-content"));
                    if (resultContent is null or { Count: 0 }) return;

                    ImmutableList<ChildPageDataItem> childPageDataItems = ImmutableList.Create<ChildPageDataItem>();
                    foreach (IWebElement element in resultContent)
                    {
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

                    if (childPageDataItems is { Count: > 0 })
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
        throw new NotImplementedException();
    }
}