using System.Web;
using Berry.Spider.Contracts;
using Berry.Spider.Core;
using Berry.Spider.Domain.Shared;
using Berry.Spider.TouTiao.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace Berry.Spider.TouTiao;

/// <summary>
/// 今日头条：问答
/// </summary>
[Dependency(ServiceLifetime.Transient), ExposeServices(typeof(ITouTiaoSpiderProvider))]
public class TouTiaoSpider4QuestionProvider : ITouTiaoSpiderProvider
{
    private ILogger<TouTiaoSpider4QuestionProvider> Logger { get; }
    private IWebElementLoadProvider WebElementLoadProvider { get; }
    private IDistributedEventBus DistributedEventBus { get; }

    private string HomePage => "https://so.toutiao.com/search?keyword={0}&pd=question&dvpf=pc";

    public TouTiaoSpider4QuestionProvider(ILogger<TouTiaoSpider4QuestionProvider> logger,
        IWebElementLoadProvider provider,
        IDistributedEventBus eventBus)
    {
        this.Logger = logger;
        this.WebElementLoadProvider = provider;
        this.DistributedEventBus = eventBus;
    }

    public async Task ExecuteAsync<T>(T request) where T : ISpiderRequest
    {
        #region 用于测试

        // while (true)
        // {
        //     var eto = new TouTiaoSpider4QuestionEto
        //     {
        //         Keyword = "写莲花外貌的句子150字？",
        //         SourceFrom = SpiderSourceFrom.TouTiao_Question,
        //         Items = new List<SpiderChildPageDataItem>
        //         {
        //             new SpiderChildPageDataItem
        //             {
        //                 Title = "春天的句子？",
        //                 Href =
        //                     "https://so.toutiao.com/s/search_wenda_pc/list/?qid=6959168672381092127&enter_answer_id=6959174410759323942&enter_from=search_result"
        //             }
        //         }
        //     };
        //     await this.DistributedEventBus.PublishAsync(eto);
        //     
        //     Thread.Sleep(5000);
        // }

        #endregion

        try
        {
            string targetUrl = string.Format(this.HomePage, request.Keyword);
            await this.WebElementLoadProvider.InvokeAsync(
                targetUrl,
                drv => drv.FindElement(By.ClassName("s-result-list")),
                async root =>
                {
                    var resultContent = root.FindElements(By.ClassName("result-content"));
                    this.Logger.LogInformation("总共获取到记录：" + resultContent.Count);

                    if (resultContent.Count > 0)
                    {
                        var eto = new TouTiaoSpider4QuestionEto() {Keyword = request.Keyword, Title = request.Keyword};

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
                                Uri jumpUri = new Uri(HttpUtility.UrlDecode(jumpUrl));
                                if (jumpUri.Host.Contains("toutiao"))
                                {
                                    eto.Items.Add(new SpiderChildPageDataItem
                                    {
                                        Title = text,
                                        Href = jumpUri.ToString()
                                    });

                                    this.Logger.LogInformation(text + "  ---> " + href);
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
}