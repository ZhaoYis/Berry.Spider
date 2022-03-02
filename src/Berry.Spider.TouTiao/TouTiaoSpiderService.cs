using System.Drawing;
using Berry.Spider.Core;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Volo.Abp.EventBus.Distributed;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace Berry.Spider.TouTiao;

/// <summary>
/// 今日头条爬虫
/// </summary>
public class TouTiaoSpiderService : SpiderBase, ITouTiaoSpiderService
{
    private ILogger<TouTiaoSpiderService> Logger { get; }
    private ISeleniumProxyProvider SeleniumProxyProvider { get; }
    private IDistributedEventBus DistributedEventBus { get; }

    public TouTiaoSpiderService(ILogger<TouTiaoSpiderService> logger, ISeleniumProxyProvider proxyProvider,
        IDistributedEventBus eventBus) : base("https://so.toutiao.com/search?keyword={0}&pd=information&dvpf=pc")
    {
        this.Logger = logger;
        this.SeleniumProxyProvider = proxyProvider;
        this.DistributedEventBus = eventBus;
    }

    protected override void Init()
    {
        new DriverManager().SetUpDriver(new ChromeConfig());
        base.Init();
    }

    /// <summary>
    /// 执行爬虫
    /// </summary>
    public override async Task ExecuteAsync()
    {
        ChromeOptions options = new ChromeOptions();
        options.Proxy = await this.SeleniumProxyProvider.GetProxyAsync();

        using (var driver = new ChromeDriver("/usr/local/webdriver", options))
        {
            string keyword = "描写荷花的句子";
            driver.Navigate().GoToUrl(string.Format(this.HomePageUrl, keyword));

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

            // var searchBox = driver.FindElement(By.XPath("//*[@id='root']/div/div[4]/div/div[1]/input"));
            // var searchButton = driver.FindElement(By.XPath("//*[@id='root']/div/div[4]/div/div[1]/button"));
            //
            // searchBox.SendKeys("描写春天的句子");
            // searchButton.Click();

            // var allWindowHandles = driver.WindowHandles;
            // driver.SwitchTo().Window(allWindowHandles.Last());
            // current = driver.CurrentWindowHandle;
            // Console.WriteLine("当前窗口句柄：" + current);

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
                TouTiaoSpiderEto eto = new TouTiaoSpiderEto {Keyword = keyword};

                foreach (IWebElement element in resultContent)
                {
                    try
                    {
                        var a = element.FindElement(By.TagName("a"));
                        if (a != null)
                        {
                            string text = a.Text;
                            string href = a.GetAttribute("href");
                            this.Logger.LogInformation(text + "  ---> " + href);

                            eto.Items.Add(new TouTiaoDataItem
                            {
                                Title = text,
                                Href = href
                            });
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

            // driver.Quit();
        }
    }
}