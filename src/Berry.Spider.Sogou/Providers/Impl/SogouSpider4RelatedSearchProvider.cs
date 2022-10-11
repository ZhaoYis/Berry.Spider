using Berry.Spider.Abstractions;
using Berry.Spider.Core;
using Berry.Spider.Domain;
using Berry.Spider.FreeRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using Volo.Abp.EventBus.Distributed;

namespace Berry.Spider.Sogou;

/// <summary>
/// 搜狗：相关推荐
/// </summary>
[Spider(SpiderSourceFrom.Sogou_Related_Search)]
public class SogouSpider4RelatedSearchProvider : ProviderBase<SogouSpider4RelatedSearchProvider>, ISpiderProvider
{
    private IWebElementLoadProvider WebElementLoadProvider { get; }
    private ITextAnalysisProvider TextAnalysisProvider { get; }
    private IDistributedEventBus DistributedEventBus { get; }
    private IRedisService RedisService { get; }
    private ISpiderTitleContentRepository SpiderRepository { get; }

    private string HomePage => "https://sogou.com";

    public SogouSpider4RelatedSearchProvider(ILogger<SogouSpider4RelatedSearchProvider> logger,
        IWebElementLoadProvider provider,
        IServiceProvider serviceProvider,
        IDistributedEventBus eventBus,
        IRedisService redisService,
        ISpiderTitleContentRepository repository) : base(logger)
    {
        this.WebElementLoadProvider = provider;
        this.TextAnalysisProvider = serviceProvider.GetRequiredService<SogouRelatedSearchTextAnalysisProvider>();
        this.DistributedEventBus = eventBus;
        this.RedisService = redisService;
        this.SpiderRepository = repository;
    }

    /// <summary>
    /// 向队列推送源数据
    /// </summary>
    /// <returns></returns>
    public async Task PushAsync<T>(T push) where T : class, ISpiderPushEto
    {
        await this.BloomCheckAsync(push.Keyword, async () => { await this.DistributedEventBus.PublishAsync(push); });
    }

    /// <summary>
    /// 二次重复性校验
    /// </summary>
    /// <returns></returns>
    protected override async Task<bool> DuplicateCheckAsync(string keyword)
    {
        bool result = await this.RedisService.SetAsync(GlobalConstants.SPIDER_KEYWORDS_KEY, keyword);
        return result;
    }

    /// <summary>
    /// 执行获取一级页面数据任务
    /// </summary>
    public async Task ExecuteAsync<T>(T request) where T : class, ISpiderRequest
    {
        try
        {
            //获取url地址
            string realUrl = await this.WebElementLoadProvider.AutoClickAsync(this.HomePage, request.Keyword,
                By.Id("query"),
                By.Id("stb"));
            if (string.IsNullOrWhiteSpace(realUrl)) return;

            await this.WebElementLoadProvider.InvokeAsync(
                realUrl,
                drv =>
                {
                    try
                    {
                        return drv.FindElement(By.Id("hint_container"));
                    }
                    catch (Exception e)
                    {
                        return null;
                    }
                },
                async root =>
                {
                    if (root == null) return;

                    var resultContent = root.FindElements(By.TagName("a"));
                    this.Logger.LogInformation("总共获取到记录：" + resultContent.Count);

                    if (resultContent.Count > 0)
                    {
                        var eto = new SogouSpider4RelatedSearchPullEto
                            { Keyword = request.Keyword, Title = request.Keyword };

                        foreach (IWebElement element in resultContent)
                        {
                            string text = element.Text;
                            string href = element.GetAttribute("href");

                            eto.Items.Add(new ChildPageDataItem
                            {
                                Title = text,
                                Href = href
                            });

                            this.Logger.LogInformation(text + "  ---> " + href);
                        }

                        if (eto.Items.Any())
                        {
                            //此处不做消息队列发送，直接存储到数据库
                            await this.HandleEventAsync(eto);
                            this.Logger.LogInformation("数据保存成功...");
                        }
                    }
                });
        }
        catch (Exception exception)
        {
            this.Logger.LogException(exception);
        }
        finally
        {
            //ignore..
        }
    }

    /// <summary>
    /// 执行根据一级页面采集到的地址获取二级页面具体目标数据任务
    /// </summary>
    public Task HandleEventAsync<T>(T eventData) where T : class, ISpiderPullEto
    {
        try
        {
            List<SpiderTitleContent> contents = new List<SpiderTitleContent>();
            foreach (var item in eventData.Items)
            {
                var content = new SpiderTitleContent(item.Title, item.Href, eventData.SourceFrom);
                contents.Add(content);
            }

            return this.SpiderRepository.InsertManyAsync(contents);
        }
        catch (Exception exception)
        {
            this.Logger.LogException(exception);
        }
        finally
        {
            //ignore..
        }

        return Task.CompletedTask;
    }
}