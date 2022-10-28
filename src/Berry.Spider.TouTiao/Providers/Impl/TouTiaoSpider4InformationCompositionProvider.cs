using System.Web;
using Berry.Spider.Abstractions;
using Berry.Spider.Contracts;
using Berry.Spider.Core;
using Berry.Spider.Domain;
using Berry.Spider.FreeRedis;
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
    private IRedisService RedisService { get; }
    private IDistributedEventBus DistributedEventBus { get; }
    private ISpiderContentRepository SpiderRepository { get; }
    private SpiderDomainService SpiderDomainService { get; }
    private IOptionsSnapshot<SpiderOptions> Options { get; }

    private string HomePage => "https://so.toutiao.com/search?keyword={0}&pd=information&dvpf=pc";

    public TouTiaoSpider4InformationCompositionProvider(ILogger<TouTiaoSpider4InformationCompositionProvider> logger,
        IGuidGenerator guidGenerator,
        IWebElementLoadProvider provider,
        IRedisService redisService,
        IDistributedEventBus eventBus,
        ISpiderContentRepository spiderRepository,
        SpiderDomainService spiderDomainService,
        IOptionsSnapshot<SpiderOptions> options) : base(logger)
    {
        this.GuidGenerator = guidGenerator;
        this.WebElementLoadProvider = provider;
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
        await this.CheckAsync(push.Keyword, async () => { await this.DistributedEventBus.PublishAsync(push); },
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
        try
        {
            string targetUrl = string.Format(this.HomePage, request.Keyword);
            await this.WebElementLoadProvider.InvokeAsync(
                targetUrl,
                drv =>
                {
                    try
                    {
                        return drv.FindElement(By.ClassName("s-result-list"));
                    }
                    catch (Exception e)
                    {
                        return null;
                    }
                },
                async root =>
                {
                    if (root == null) return;

                    var resultContent = root.FindElements(By.ClassName("result-content"));
                    this.Logger.LogInformation("总共获取到记录：" + resultContent.Count);

                    if (resultContent.Count > 0)
                    {
                        var eto = new TouTiaoSpider4InformationCompositionPullEto
                            {Keyword = request.Keyword, Title = request.Keyword};

                        foreach (IWebElement element in resultContent)
                        {
                            var a = element.FindElement(By.TagName("a"));
                            if (a != null)
                            {
                                string text = a.Text;
                                string href = a.GetAttribute("href");

                                //去获取so.toutiao.com、tsearch.toutiaoapi.com的记录
                                Uri sourceUri = new Uri(href);
                                //?url=https://so.toutiao.com/s/search_wenda_pc/list/?qid=6959168672381092127&enter_answer_id=6959174410759323942&enter_from=search_result&aid=4916&jtoken=c47d820935b56f1e45ae0f2b729ffa52df0fa9ae4d13f409a370b005eb0492689aeea6f8881750a45f53aaca866c7950849eb3e24f7d4db160483899ca0389bd
                                string jumpUrl = sourceUri.Query.Substring(5);
                                if (jumpUrl.StartsWith("http") || jumpUrl.StartsWith("https"))
                                {
                                    Uri jumpUri = new Uri(HttpUtility.UrlDecode(jumpUrl));
                                    if (jumpUri.Host.Contains("toutiao"))
                                    {
                                        eto.Items.Add(new ChildPageDataItem
                                        {
                                            Title = text,
                                            Href = jumpUri.ToString()
                                        });

                                        this.Logger.LogInformation(text + "  ---> " + href);
                                    }
                                }
                            }
                        }

                        if (eto.Items.Any())
                        {
                            await this.DistributedEventBus.PublishAsync(eto);
                            this.Logger.LogInformation("事件发布成功，等待消费...");
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
                    drv =>
                    {
                        try
                        {
                            return drv.FindElement(By.ClassName("article-content"));
                        }
                        catch (Exception e)
                        {
                            return null;
                        }
                    },
                    async root =>
                    {
                        if (root == null) return;

                        var resultContent = root.FindElement(By.TagName("article"));
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
        finally
        {
            //ignore..
        }
    }
}