using System.Drawing;
using Berry.Spider.Core;
using Berry.Spider.Proxy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
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
    private ILogger<TouTiaoSpiderService> Logger { get; set; }
    private IHttpProxy HttpProxy { get; }
    private IDistributedEventBus DistributedEventBus { get; }

    protected override string HomePageUrl => "https://so.toutiao.com/search?keyword={0}&pd=information&dvpf=pc";

    public TouTiaoSpiderService(IHttpProxy httpProxy, IDistributedEventBus eventBus)
    {
        this.Logger = NullLogger<TouTiaoSpiderService>.Instance;
        this.HttpProxy = httpProxy;
        this.DistributedEventBus = eventBus;
    }

    /// <summary>
    /// 执行爬虫
    /// </summary>
    public override async Task ExecuteAsync()
    {
        new DriverManager().SetUpDriver(new ChromeConfig());

        var options = new ChromeOptions();
        OpenQA.Selenium.Proxy proxy = new OpenQA.Selenium.Proxy();
        proxy.Kind = ProxyKind.Manual;
        proxy.IsAutoDetect = false;
        proxy.HttpProxy = await this.HttpProxy.GetProxyUriAsync();
        options.Proxy = proxy;

        using (var driver = new ChromeDriver("/usr/local/webdriver", options))
        {
            driver.Navigate().GoToUrl(string.Format(this.HomePageUrl, "死神都不敢惹的五大星座"));

            string title = driver.Title;
            string url = driver.Url;
            string current = driver.CurrentWindowHandle;
            Console.WriteLine("当前窗口句柄：" + current);

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
            Console.WriteLine("总共获取到记录：" + resultContent.Count);

            if (resultContent.Count > 0)
            {
                TouTiaoSpiderEto eto = new TouTiaoSpiderEto();

                foreach (IWebElement element in resultContent)
                {
                    try
                    {
                        var a = element.FindElement(By.TagName("a"));
                        if (a != null)
                        {
                            string text = a.Text;
                            string href = a.GetAttribute("href");

                            Console.WriteLine(text + "  ---> " + href);

                            eto.Items.Add(new TouTiaoDataItem
                            {
                                Title = text,
                                Href = href
                            });
                        }
                    }
                    catch (NoSuchElementException elementException)
                    {
                        Console.WriteLine(elementException.Message);
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception.Message);
                    }
                }

                if (eto.Items.Any())
                {
                    await this.DistributedEventBus.PublishAsync(eto);
                }
            }

            // driver.Quit();
        }
    }
}