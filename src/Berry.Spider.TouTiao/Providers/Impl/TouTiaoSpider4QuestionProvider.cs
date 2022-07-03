using Berry.Spider.Core;
using Berry.Spider.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using System.Web;
using Berry.Spider.Contracts;
using Microsoft.Extensions.Options;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Timing;

namespace Berry.Spider.TouTiao;

/// <summary>
/// 今日头条：问答
/// </summary>
public class TouTiaoSpider4QuestionProvider : ITouTiaoSpiderProvider
{
    private ILogger<TouTiaoSpider4QuestionProvider> Logger { get; }
    private IWebElementLoadProvider WebElementLoadProvider { get; }
    private ITextAnalysisProvider TextAnalysisProvider { get; }
    private IImageResourceProvider ImageResourceProvider { get; }
    private IFormattingTitleProvider FormattingTitleProvider { get; }
    private ISpiderContentRepository SpiderRepository { get; }
    private IClock Clock { get; }
    private IDistributedEventBus DistributedEventBus { get; }
    private IOptionsSnapshot<SpiderOptions> Options { get; }

    private string HomePage => "https://so.toutiao.com/search?keyword={0}&pd=question&dvpf=pc";

    public TouTiaoSpider4QuestionProvider(ILogger<TouTiaoSpider4QuestionProvider> logger,
        IWebElementLoadProvider provider,
        IServiceProvider serviceProvider,
        IImageResourceProvider imageResourceProvider,
        IFormattingTitleProvider formattingTitleProvider,
        ISpiderContentRepository repository,
        IClock clock,
        IDistributedEventBus eventBus,
        IOptionsSnapshot<SpiderOptions> options)
    {
        this.Logger = logger;
        this.WebElementLoadProvider = provider;
        this.TextAnalysisProvider = serviceProvider.GetRequiredService<TouTiaoQuestionTextAnalysisProvider>();
        this.ImageResourceProvider = imageResourceProvider;
        this.FormattingTitleProvider = formattingTitleProvider;
        this.SpiderRepository = repository;
        this.Clock = clock;
        this.DistributedEventBus = eventBus;
        this.Options = options;
    }

    public async Task ExecuteAsync<T>(T request) where T : ISpiderRequest
    {
        try
        {
            bool isExisted =
                await this.SpiderRepository.CountAsync(c => c.Title == request.Keyword && c.Published == 0) > 0;
            if (isExisted) return;

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
                        var eto = new TouTiaoSpider4QuestionEto {Keyword = request.Keyword, Title = request.Keyword};

                        foreach (IWebElement element in resultContent)
                        {
                            //TODO:只取 大家都在问 的部分

                            //https://so.toutiao.com/search/jump?url=https://so.toutiao.com/s/search_wenda_pc/list/?qid=6959168672381092127&enter_answer_id=6959174410759323942&enter_from=search_result&aid=4916&jtoken=c47d820935b56f1e45ae0f2b729ffa52df0fa9ae4d13f409a370b005eb0492689aeea6f8881750a45f53aaca866c7950849eb3e24f7d4db160483899ca0389bd
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

    public async Task HandleEventAsync<T>(T eventData) where T : ISpiderPullEto
    {
        try
        {
            bool isExisted =
                await this.SpiderRepository.CountAsync(c => c.Title == eventData.Title && c.Published == 0) > 0;
            if (isExisted) return;

            List<string> contentItems = new List<string>();

            foreach (var item in eventData.Items)
            {
                await this.WebElementLoadProvider.InvokeAsync(
                    item.Href,
                    drv =>
                    {
                        try
                        {
                            return drv.FindElement(By.ClassName("s-container"));
                        }
                        catch (Exception e)
                        {
                            return null;
                        }
                    },
                    async root =>
                    {
                        if (root == null) return;

                        var resultContent = root.FindElements(By.ClassName("list"));
                        if (resultContent.Count > 0)
                        {
                            foreach (IWebElement element in resultContent)
                            {
                                var answerList = element
                                    .FindElements(By.TagName("div"))
                                    .Where(c => c.GetAttribute("class").StartsWith("answer_layout_wrapper_"))
                                    .ToList();
                                if (answerList.Any())
                                {
                                    foreach (IWebElement answer in answerList)
                                    {
                                        if (answer != null && !string.IsNullOrWhiteSpace(answer.Text))
                                        {
                                            //解析内容
                                            var list = await this.TextAnalysisProvider.InvokeAsync(answer.Text);
                                            if (list.Count > 0)
                                            {
                                                contentItems.AddRange(list);
                                                this.Logger.LogInformation("总共获取到记录：" + list.Count);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                );
            }

            //去重
            contentItems = contentItems.Distinct().ToList();
            if (contentItems.Count >= this.Options.Value.MinRecords)
            {
                //打乱
                contentItems.RandomSort();

                string mainContent;
                if (this.Options.Value.IsInsertImage)
                {
                    if (this.Options.Value.IsRandomInsertImage)
                    {
                        mainContent = this.Clock.Now.Hour % 2 == 0 ? contentItems.BuildMainContent(this.ImageResourceProvider) : contentItems.BuildMainContent();
                    }
                    else
                    {
                        mainContent = contentItems.BuildMainContent(this.ImageResourceProvider);
                    }
                }
                else
                {
                    mainContent = contentItems.BuildMainContent();
                }

                if (!string.IsNullOrEmpty(mainContent))
                {
                    //重写Title
                    string title = this.FormattingTitleProvider.Format(eventData.Title, contentItems.Count);

                    //组装数据
                    var content = new SpiderContent(title, mainContent, eventData.SourceFrom);
                    await this.SpiderRepository.InsertAsync(content);

                    this.Logger.LogInformation("落库成功，标题：" + eventData.Title + "，共计：" + contentItems.Count + "条记录");
                }
            }
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