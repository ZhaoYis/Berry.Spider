using System.Collections.Immutable;
using Berry.Spider.Application.Contracts;
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
/// 今日头条：头条_微头条
/// </summary>
[SpiderService([SpiderSourceFrom.TouTiao_WeiTouTiao])]
public class TouTiaoSpider4ArticleProvider : ProviderBase<TouTiaoSpider4ArticleProvider>, ISpiderProvider
{
    private IGuidGenerator GuidGenerator { get; }
    private IWebElementLoadProvider WebElementLoadProvider { get; }
    private IResolveJumpUrlProvider ResolveJumpUrlProvider { get; }
    private IRedisService RedisService { get; }
    private ISpiderContentRepository SpiderRepository { get; }
    private IEventBusPublisher DistributedEventBus { get; }
    private SpiderOptions Options { get; }

    private string HomePage => "https://so.toutiao.com/search?keyword={0}&pd=weitoutiao&dvpf=pc";

    public TouTiaoSpider4ArticleProvider(ILogger<TouTiaoSpider4ArticleProvider> logger,
        IGuidGenerator guidGenerator,
        IWebElementLoadProvider provider,
        IServiceProvider serviceProvider,
        ISpiderContentRepository spiderRepository,
        IRedisService redisService,
        IEventBusPublisher eventBus,
        IOptionsSnapshot<SpiderOptions> options) : base(logger)
    {
        this.GuidGenerator = guidGenerator;
        this.WebElementLoadProvider = provider;
        this.ResolveJumpUrlProvider = serviceProvider.GetRequiredService<TouTiaoResolveJumpUrlProvider>();
        this.RedisService = redisService;
        this.SpiderRepository = spiderRepository;
        this.DistributedEventBus = eventBus;
        this.Options = options.Value;
    }

    /// <summary>
    /// 向队列推送源数据
    /// </summary>
    /// <returns></returns>
    public async Task PushAsync(ISpiderPushToQueueDto dto)
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
                        this.Logger.LogInformation("通道：{Description}，关键字：{Keyword}，一级页面：{Count}条", eventData.SourceFrom.GetDescription(), eventData.Keyword, childPageDataItems.Count.ToString());

                        var eto = eventData.SourceFrom.TryCreateEto(EtoType.Pull, eventData.SourceFrom,
                            eventData.Keyword, eventData.Keyword, childPageDataItems.ToList(), eventData.TraceCode,
                            eventData.IdentityId);
                        await this.DistributedEventBus.PublishAsync(eto.TryGetRoutingKey(), eto);
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
            string groupId = this.GuidGenerator.Create().ToString("N");
            ImmutableList<SpiderContent> contentItems = ImmutableList.Create<SpiderContent>();
            await this.WebElementLoadProvider.BatchInvokeAsync(
                eventData.Items.DistinctBy(x => x.Title).ToDictionary(k => k.Title, v => v.Href),
                drv => drv.FindElement(By.CssSelector(".article-content")),
                async (root, keyword) =>
                {
                    if (root == null) return;

                    var resultContent = root.TryFindElement(By.TagName("article"));
                    if (resultContent != null)
                    {
                        string content = resultContent.Text;
                        if (!string.IsNullOrEmpty(content))
                        {
                            SpiderContent spiderContent = new SpiderContent(keyword.ToString(), content, groupId, eventData.SourceFrom);
                            spiderContent.SetTraceCodeIfNotNull(eventData.TraceCode);
                            spiderContent.SetIdentityIdIfNotNull(eventData.IdentityId);
                            contentItems = contentItems.Add(spiderContent);
                        }
                    }

                    await Task.Delay(20).ConfigureAwait(false);
                }
            );

            //去重
            List<SpiderContent> todoSaveContentItems = contentItems.Where(c => !string.IsNullOrEmpty(c.Content)).ToList();
            await this.SpiderRepository.InsertManyAsync(todoSaveContentItems);
        }
        catch (Exception exception)
        {
            this.Logger.LogException(exception);
        }
    }
}