using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Drawing;
using Volo.Abp;

namespace Berry.Spider.Core;

public class WebElementLoadProvider : IWebElementLoadProvider
{
    private ILogger<WebElementLoadProvider> Logger { get; }
    private IWebDriverProvider WebDriverProvider { get; }
    private IHumanMachineVerificationInterceptorProvider InterceptorProvider { get; }

    public WebElementLoadProvider(ILogger<WebElementLoadProvider> logger,
        IWebDriverProvider webDriverProvider,
        IHumanMachineVerificationInterceptorProvider interceptorProvider)
    {
        this.Logger = logger;
        this.WebDriverProvider = webDriverProvider;
        this.InterceptorProvider = interceptorProvider;
    }

    public async Task InvokeAsync(string targetUrl, Func<IWebDriver, IWebElement?> selector,
        Func<IWebElement?, Task> executor)
    {
        //检查是否处于人机验证资源锁定阶段
        if (await this.InterceptorProvider.IsLockedAsync())
        {
            throw new BusinessException("人机验证资源锁定中，请稍后再试~");
        }

        using (var driver = await this.WebDriverProvider.GetAsync())
        {
            try
            {
                driver.Navigate().GoToUrl(targetUrl);

                //人机验证拦截
                await this.InterceptorProvider.InvokeAsync(driver);

                {
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

                    var page = driver.PageSource;

                    IWebElement? webElement = wait.Until(selector);
                    await executor.Invoke(webElement);
                }
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