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
[SpiderService(new[] {SpiderSourceFrom.TouTiao_Information})]
public class TouTiaoSpider4InformationProvider : ProviderBase<TouTiaoSpider4InformationProvider>, ISpiderProvider
{
    private IWebElementLoadProvider WebElementLoadProvider { get; }
    private IResolveJumpUrlProvider ResolveJumpUrlProvider { get; }
    private ISpiderContentKeywordRepository SpiderKeywordRepository { get; }
    private IRedisService RedisService { get; }
    private IEventBusPublisher DistributedEventBus { get; }
    private IOptionsSnapshot<SpiderOptions> Options { get; }

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
        this.Options = options;
    }

    /// <summary>
    /// 向队列推送源数据
    /// </summary>
    /// <returns></returns>
    public async Task PushAsync(string keyword, SpiderSourceFrom from)
    {
        TouTiaoSpider4InformationPushEto push = new TouTiaoSpider4InformationPushEto
        {
            SourceFrom = from,
            Keyword = keyword
        };

        await this.CheckAsync(push.Keyword, from, async () =>
            {
                await this.DistributedEventBus.PublishAsync(push.TryGetRoutingKey(), push);
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
                        },
                        async (element, token) =>
                        {
                            var a = element.TryFindElement(By.TagName("a"));
                            if (a != null)
                            {
                                string text = a.Text;
                                string href = a.GetAttribute("href");

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
                        var eto = new TouTiaoSpider4InformationPullEto
                        {
                            Keyword = eventData.Keyword,
                            Title = eventData.Keyword,
                            Items = childPageDataItems.ToList()
                        };

                        await this.DistributedEventBus.PublishAsync(eto.TryGetRoutingKey(), eto);

                        //保存采集到的标题
                        List<SpiderContent_Keyword> list = eto.Items.Select(item => new SpiderContent_Keyword(item.Title, eto.SourceFrom)).ToList();
                        await this.SpiderKeywordRepository.InsertManyAsync(list);
                    }
                }
            });
    }

    /// <summary>
    /// 执行根据一级页面采集到的地址获取二级页面具体目标数据任务
    /// </summary>
    /// <returns></returns>
    public Task HandlePullEventAsync<T>(T eventData) where T : class, ISpiderPullEto
    {
        throw new NotImplementedException();
    }
}