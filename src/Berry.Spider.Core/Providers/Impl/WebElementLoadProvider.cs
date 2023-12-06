using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Drawing;

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

    public async Task InvokeAsync(string targetUrl,
        string keyword,
        Func<IWebDriver, IWebElement?> selector,
        Func<IWebElement?, string, Task> executor)
    {
        try
        {
            using IWebDriver driver = await this.WebDriverProvider.GetAsync();
            driver.Navigate().GoToUrl(targetUrl);

            //获取跳转后url
            string title = driver.Title;
            string page = driver.PageSource;
            string url = driver.Url;
            string current = driver.CurrentWindowHandle;

            if (string.IsNullOrEmpty(title)) return;
            this.Logger.LogInformation("[Void]窗口句柄：{0}，关键字：{1}，地址：{2}", current, title, url);

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
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(WebDriverTimeoutException), typeof(NotFoundException));

            IWebElement? webElement = wait.Until(selector);
            await executor.Invoke(webElement, keyword);
        }
        catch (Exception exception)
        {
            this.Logger.LogException(exception);
            await executor.Invoke(null, keyword);
        }
    }

    public async Task BatchInvokeAsync(IDictionary<string, string> keywordList,
        Func<IWebDriver, IWebElement?> selector,
        Func<IWebElement?, string, Task> executor)
    {
        try
        {
            using IWebDriver driver = await this.WebDriverProvider.GetAsync();
            foreach (KeyValuePair<string, string> pair in keywordList)
            {
                string keyword = pair.Key;
                string targetUrl = pair.Value;
                driver.Navigate().GoToUrl(targetUrl);

                //获取跳转后url
                string title = driver.Title;
                string page = driver.PageSource;
                string url = driver.Url;
                string current = driver.CurrentWindowHandle;

                if (string.IsNullOrEmpty(title)) return;
                this.Logger.LogInformation("[Void]窗口句柄：{0}，关键字：{1}，地址：{2}", current, title, url);

                WebDriverWait wait = new WebDriverWait(driver, timeout: TimeSpan.FromSeconds(30))
                {
                    PollingInterval = TimeSpan.FromSeconds(5),
                };
                wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(WebDriverTimeoutException), typeof(NotFoundException));

                IWebElement? webElement = wait.Until(selector);
                await executor.Invoke(webElement, keyword);

                await Task.Delay(20);
            }
        }
        catch (Exception exception)
        {
            this.Logger.LogException(exception);
            await executor.Invoke(null, "");
        }
    }

    public async Task<T?> InvokeAndReturnAsync<T>(string targetUrl,
        string keyword,
        Func<IWebDriver, IWebElement?> selector,
        Func<IWebElement?, string, Task<T>> executor)
    {
        try
        {
            using IWebDriver driver = await this.WebDriverProvider.GetAsync();
            driver.Navigate().GoToUrl(targetUrl);

            //获取跳转后url
            string title = driver.Title;
            string page = driver.PageSource;
            string url = driver.Url;
            string current = driver.CurrentWindowHandle;

            if (string.IsNullOrEmpty(title)) return default;
            this.Logger.LogInformation("[T]窗口句柄：{0}，关键字：{1}，地址：{2}", current, title, url);

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
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(WebDriverTimeoutException), typeof(NotFoundException));

            IWebElement? webElement = wait.Until(selector);
            T result = await executor.Invoke(webElement, keyword);
            return result;
        }
        catch (Exception exception)
        {
            this.Logger.LogException(exception);
            await executor.Invoke(null, keyword);
            return await Task.FromResult(default(T));
        }
    }

    public async Task<string> AutoClickAsync(string targetUrl,
        string keyword,
        By inputBox,
        By submitBtn)
    {
        try
        {
            using IWebDriver driver = await this.WebDriverProvider.GetAsync();
            driver.Navigate().GoToUrl(targetUrl);
            //获取输入框
            driver.FindElement(inputBox).SendKeys(keyword);
            //点击按钮
            driver.FindElement(submitBtn).Click();

            //获取跳转后url
            string title = driver.Title;
            string url = driver.Url;
            string current = driver.CurrentWindowHandle;

            if (string.IsNullOrEmpty(title)) return string.Empty;
            this.Logger.LogInformation("[AC]窗口句柄：{0}，关键字：{1}，地址：{2}", current, title, url);

            // 隐式等待
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            // 设置Cookie
            // driver.Manage().Cookies.AddCookie(new Cookie("key", "value"));
            // 将窗口移动到主显示器的左上角
            driver.Manage().Window.Position = new Point(0, 0);

            return url;
        }
        catch (Exception exception)
        {
            this.Logger.LogException(exception);
        }

        return string.Empty;
    }

    public async Task AutoClickAndInvokeAsync(string targetUrl,
        string keyword,
        By inputBox,
        By submitBtn,
        Func<IWebDriver, IWebElement?> selector,
        Func<IWebElement?, string, Task> executor)
    {
        try
        {
            using IWebDriver driver = await this.WebDriverProvider.GetAsync();
            driver.Navigate().GoToUrl(targetUrl);
            //获取输入框
            driver.FindElement(inputBox).SendKeys(keyword);
            //点击按钮
            driver.FindElement(submitBtn).Click();

            driver.Navigate().GoToUrl(driver.Url);
            WebDriverWait wait = new WebDriverWait(driver, timeout: TimeSpan.FromSeconds(30))
            {
                PollingInterval = TimeSpan.FromSeconds(5),
            };
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(WebDriverTimeoutException), typeof(NotFoundException));

            IWebElement? webElement = wait.Until(selector);
            await executor.Invoke(webElement, keyword);
        }
        catch (Exception exception)
        {
            this.Logger.LogException(exception);
            await executor.Invoke(null, keyword);
        }
    }
}