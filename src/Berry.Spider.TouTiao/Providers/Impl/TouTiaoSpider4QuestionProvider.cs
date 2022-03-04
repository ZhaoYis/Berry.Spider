using System.Drawing;
using System.Web;
using Berry.Spider.Contracts;
using Berry.Spider.Core;
using Berry.Spider.TouTiao.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
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
    private IWebDriverProvider WebDriverProvider { get; }
    private IDistributedEventBus DistributedEventBus { get; }

    public TouTiaoSpider4QuestionProvider(ILogger<TouTiaoSpider4QuestionProvider> logger,
        IWebDriverProvider webDriverProvider,
        IDistributedEventBus eventBus)
    {
        this.Logger = logger;
        this.WebDriverProvider = webDriverProvider;
        this.DistributedEventBus = eventBus;
    }

    public async Task ExecuteAsync()
    {
        using (var driver = await this.WebDriverProvider.GetAsync())
        {
            try
            {
                string keyword = "描写荷花的句子";
                string targetUrl = string.Format("https://so.toutiao.com/search?keyword={0}&pd=question&dvpf=pc",
                    keyword);
                driver.Navigate().GoToUrl(targetUrl);

                string title = driver.Title;
                string url = driver.Url;
                this.Logger.LogInformation("开始执行[{0}]，页面地址：{1}", title, url);

                string current = driver.CurrentWindowHandle;
                this.Logger.LogInformation("当前窗口句柄：" + current);

                // 隐式等待
                //driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                // 设置Cookie
                // driver.Manage().Cookies.AddCookie(new Cookie("key", "value"));
                // 将窗口移动到主显示器的左上角
                driver.Manage().Window.Position = new Point(0, 0);

                WebDriverWait wait = new WebDriverWait(driver, timeout: TimeSpan.FromSeconds(30))
                {
                    PollingInterval = TimeSpan.FromSeconds(5),
                };
                wait.IgnoreExceptionTypes(typeof(NoSuchElementException));

                IWebElement webElement = wait.Until(drv => drv.FindElement(By.ClassName("s-result-list")));
                var resultContent = webElement.FindElements(By.ClassName("result-content"));
                this.Logger.LogInformation("总共获取到记录：" + resultContent.Count);

                if (resultContent.Count > 0)
                {
                    var eto = new TouTiaoSpider4QuestionEto() { Keyword = keyword };

                    foreach (IWebElement element in resultContent)
                    {
                        try
                        {
                            //只取 大家都在问 的部分
                            // var span = element.FindElement(By.ClassName(
                            //     "//div[@class='cs-view cs-view-flex align-items-center flex-row cs-source-content']"));
                            // if (span != null)
                            // {
                            //     if (span.Text.Trim().Equals("大家都在问"))
                            //     {
                            //     }
                            // }

                            //https://so.toutiao.com/search/jump?url=https://so.toutiao.com/s/search_wenda_pc/list/?qid=6959168672381092127&enter_answer_id=6959174410759323942&enter_from=search_result&aid=4916&jtoken=c47d820935b56f1e45ae0f2b729ffa52df0fa9ae4d13f409a370b005eb0492689aeea6f8881750a45f53aaca866c7950849eb3e24f7d4db160483899ca0389bd
                            var a = element.FindElement(By.TagName("a"));
                            if (a != null)
                            {
                                string text = a.Text;
                                string href = a.GetAttribute("href");

                                //去获取so.toutiao.com的记录
                                Uri sourceUri = new Uri(href);
                                //?url=https://so.toutiao.com/s/search_wenda_pc/list/?qid=6959168672381092127&enter_answer_id=6959174410759323942&enter_from=search_result&aid=4916&jtoken=c47d820935b56f1e45ae0f2b729ffa52df0fa9ae4d13f409a370b005eb0492689aeea6f8881750a45f53aaca866c7950849eb3e24f7d4db160483899ca0389bd
                                string jumpUrl = sourceUri.Query.Substring(5);
                                Uri jumpUri = new Uri(HttpUtility.UrlDecode(jumpUrl));
                                if (sourceUri.Host.Equals(jumpUri.Host))
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
                        catch (WebDriverException exception)
                        {
                            this.Logger.LogException(exception);
                        }
                        catch (Exception exception)
                        {
                            this.Logger.LogException(exception);
                        }
                    }

                    if (eto.Items.Any())
                    {
                        await this.DistributedEventBus.PublishAsync(eto);
                        this.Logger.LogInformation("事件发布成功，等待消费...");
                    }
                }
            }
            finally
            {
                driver.Quit();
            }
        }
    }
}