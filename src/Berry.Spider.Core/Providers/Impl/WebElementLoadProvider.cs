using System.Drawing;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Berry.Spider.Core;

public class WebElementLoadProvider : IWebElementLoadProvider
{
    private ILogger<WebElementLoadProvider> Logger { get; }
    private IWebDriverProvider WebDriverProvider { get; }

    public WebElementLoadProvider(ILogger<WebElementLoadProvider> logger,
        IWebDriverProvider webDriverProvider)
    {
        this.Logger = logger;
        this.WebDriverProvider = webDriverProvider;
    }

    public async Task InvokeAsync(string targetUrl, Func<IWebDriver, IWebElement?> selector,
        Func<IWebElement?, Task> executor)
    {
        using (var driver = await this.WebDriverProvider.GetAsync())
        {
            try
            {
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

                IWebElement? webElement = wait.Until(selector);
                await executor.Invoke(webElement);
            }
            catch (Exception exception)
            {
                this.Logger.LogException(exception);
            }
            finally
            {
                driver.Quit();
            }
        }
    }
}