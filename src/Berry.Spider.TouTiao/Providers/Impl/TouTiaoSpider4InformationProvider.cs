using System.Drawing;
using Berry.Spider.Contracts;
using Berry.Spider.Core;
using Berry.Spider.Domain.Shared;
using Berry.Spider.TouTiao.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace Berry.Spider.TouTiao;

/// <summary>
/// 今日头条：资讯
/// </summary>
[Dependency(ServiceLifetime.Transient), ExposeServices(typeof(ITouTiaoSpiderProvider))]
public class TouTiaoSpider4InformationProvider : ITouTiaoSpiderProvider
{
    private ILogger<TouTiaoSpider4InformationProvider> Logger { get; }
    private IWebDriverProvider WebDriverProvider { get; }
    private IDistributedEventBus DistributedEventBus { get; }

    public TouTiaoSpider4InformationProvider(ILogger<TouTiaoSpider4InformationProvider> logger,
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
            string keyword = "描写荷花的句子";
            string targetUrl =
                string.Format("https://so.toutiao.com/search?keyword={0}&pd=information&dvpf=pc", keyword);
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
                var eto = new TouTiaoSpider4InformationEto() { Keyword = keyword };

                foreach (IWebElement element in resultContent)
                {
                    try
                    {
                        var a = element.FindElement(By.TagName("a"));
                        if (a != null)
                        {
                            string text = a.Text;
                            string href = a.GetAttribute("href");

                            eto.Items.Add(new SpiderChildPageDataItem
                            {
                                Title = text,
                                Href = href
                            });

                            this.Logger.LogInformation(text + "  ---> " + href);
                        }
                    }
                    catch (NoSuchElementException elementException)
                    {
                        this.Logger.LogException(elementException);
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
    }
}