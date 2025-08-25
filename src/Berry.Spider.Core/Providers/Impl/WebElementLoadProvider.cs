using System.Net;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Volo.Abp;
using Volo.Abp.Guids;

namespace Berry.Spider.Core;

public class WebElementLoadProvider : IWebElementLoadProvider
{
    private ILogger<WebElementLoadProvider> Logger { get; }
    private IWebDriverProvider WebDriverProvider { get; }
    private IGuidGenerator GuidGenerator { get; }
    private IHumanMachineVerificationInterceptorProvider InterceptorProvider { get; }

    public WebElementLoadProvider(ILogger<WebElementLoadProvider> logger,
        IWebDriverProvider webDriverProvider,
        IGuidGenerator guidGenerator,
        IHumanMachineVerificationInterceptorProvider interceptorProvider)
    {
        this.Logger = logger;
        this.WebDriverProvider = webDriverProvider;
        this.GuidGenerator = guidGenerator;
        this.InterceptorProvider = interceptorProvider;
    }

    public async Task InvokeAsync(string targetUrl,
        object state,
        Func<IWebDriver, IWebElement?> selector,
        Func<IWebElement?, object, Task> executor)
    {
        var isolationContext = this.GuidGenerator.Create().ToString();
        try
        {
            using IWebDriver driver = await this.WebDriverProvider.GetAsync(isolationContext);
            //跳转
            await driver.Navigate().GoToUrlAsync(targetUrl);
            //行为模拟
            await HumanBehavior.ScrollLikeHumanAsync(driver, steps: 2);
            string title = driver.Title;
            string page = driver.PageSource;
            string url = driver.Url;
            string current = driver.CurrentWindowHandle;

            if (string.IsNullOrEmpty(page)) return;
            this.Logger.LogInformation("[V]窗口句柄：{Current}，关键字：{Title}，地址：{Url}", current, title, WebUtility.UrlDecode(url));

            WebDriverWait wait = new WebDriverWait(driver, timeout: TimeSpan.FromSeconds(30))
            {
                PollingInterval = TimeSpan.FromSeconds(5),
            };
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(WebDriverTimeoutException), typeof(NotFoundException), typeof(StaleElementReferenceException));
            IWebElement? webElement = wait.Until(selector);
            await executor.Invoke(webElement, state);
        }
        catch (Exception exception)
        {
            this.Logger.LogException(exception);
            await executor.Invoke(null, state);
        }
        finally
        {
            this.WebDriverProvider.DeleteBrowserUserProfileDirectories(isolationContext);
        }
    }

    public async Task BatchInvokeAsync(IDictionary<string, string> keywordList,
        Func<IWebDriver, IWebElement?> selector,
        Func<IWebElement?, object, Task> executor)
    {
        var isolationContext = this.GuidGenerator.Create().ToString();
        try
        {
            using IWebDriver driver = await this.WebDriverProvider.GetAsync(isolationContext);
            foreach (KeyValuePair<string, string> pair in keywordList)
            {
                try
                {
                    string keyword = pair.Key;
                    string targetUrl = pair.Value;

                    //跳转
                    await driver.Navigate().GoToUrlAsync(targetUrl);
                    //行为模拟
                    await HumanBehavior.ScrollLikeHumanAsync(driver, steps: 2);
                    string title = driver.Title;
                    string page = driver.PageSource;
                    string url = driver.Url;
                    string current = driver.CurrentWindowHandle;

                    if (string.IsNullOrEmpty(page)) return;
                    this.Logger.LogInformation("[BV]窗口句柄：{Current}，关键字：{Title}，地址：{Url}", current, title, WebUtility.UrlDecode(url));

                    WebDriverWait wait = new WebDriverWait(driver, timeout: TimeSpan.FromSeconds(30))
                    {
                        PollingInterval = TimeSpan.FromSeconds(5),
                    };
                    wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(WebDriverTimeoutException), typeof(NotFoundException), typeof(StaleElementReferenceException));
                    IWebElement? webElement = wait.Until(selector);
                    await executor.Invoke(webElement, keyword);
                }
                finally
                {
                    await Task.Delay(RandomHelper.GetRandom(10, 20)).ConfigureAwait(false);
                }
            }
        }
        catch (Exception exception)
        {
            this.Logger.LogException(exception);
            await executor.Invoke(null, "");
        }
        finally
        {
            this.WebDriverProvider.DeleteBrowserUserProfileDirectories(isolationContext);
        }
    }

    public async Task<T?> InvokeAndReturnAsync<T>(string targetUrl,
        object state,
        Func<IWebDriver, IWebElement?> selector,
        Func<IWebElement?, object, Task<T>> executor)
    {
        var isolationContext = this.GuidGenerator.Create().ToString();
        try
        {
            //检查是否处于人机验证资源锁定阶段
            if (await this.InterceptorProvider.IsLockedAsync(targetUrl)) return default(T);

            using IWebDriver driver = await this.WebDriverProvider.GetAsync(isolationContext);
            //跳转
            await driver.Navigate().GoToUrlAsync(targetUrl);
            //行为模拟
            await HumanBehavior.ScrollLikeHumanAsync(driver, steps: 2);
            string title = driver.Title;
            string page = driver.PageSource;
            string url = driver.Url;
            string current = driver.CurrentWindowHandle;

            if (string.IsNullOrEmpty(page)) return default;
            this.Logger.LogInformation("[T]窗口句柄：{Current}，关键字：{Title}，地址：{Url}", current, title, WebUtility.UrlDecode(url));

            //人机验证拦截
            if (await this.InterceptorProvider.LockedAsync(targetUrl, url)) return default(T);

            WebDriverWait wait = new WebDriverWait(driver, timeout: TimeSpan.FromSeconds(30))
            {
                PollingInterval = TimeSpan.FromSeconds(5),
            };
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(WebDriverTimeoutException), typeof(NotFoundException), typeof(StaleElementReferenceException));
            IWebElement? webElement = wait.Until(selector);
            T result = await executor.Invoke(webElement, state);
            return result;
        }
        catch (Exception exception)
        {
            this.Logger.LogException(exception);
            await executor.Invoke(null, state);
        }
        finally
        {
            this.WebDriverProvider.DeleteBrowserUserProfileDirectories(isolationContext);
        }

        return default(T);
    }

    public async Task<string> AutoClickAsync(string targetUrl,
        object state,
        By inputBox,
        By submitBtn)
    {
        var isolationContext = this.GuidGenerator.Create().ToString();
        try
        {
            //检查是否处于人机验证资源锁定阶段
            if (await this.InterceptorProvider.IsLockedAsync(targetUrl)) return string.Empty;

            using IWebDriver driver = await this.WebDriverProvider.GetAsync(isolationContext);
            //隐式等待
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            //跳转
            await driver.Navigate().GoToUrlAsync(targetUrl);
            //行为模拟
            await HumanBehavior.ScrollLikeHumanAsync(driver, steps: 2);
            //获取输入框
            driver.FindElement(inputBox).SendKeys(state.ToString());
            //点击按钮
            driver.FindElement(submitBtn).Click();

            //获取跳转后url
            string title = driver.Title;
            string page = driver.PageSource;
            string url = driver.Url;
            string current = driver.CurrentWindowHandle;

            if (string.IsNullOrEmpty(page)) return string.Empty;
            this.Logger.LogInformation("[AC]窗口句柄：{Current}，关键字：{Title}，地址：{Url}", current, title, WebUtility.UrlDecode(url));

            //人机验证拦截
            if (await this.InterceptorProvider.LockedAsync(targetUrl, url)) return string.Empty;

            return url;
        }
        catch (Exception exception)
        {
            this.Logger.LogException(exception);
        }
        finally
        {
            this.WebDriverProvider.DeleteBrowserUserProfileDirectories(isolationContext);
        }

        return string.Empty;
    }

    public async Task AutoClickAndInvokeAsync(string targetUrl,
        object state,
        By inputBox,
        By submitBtn,
        Func<IWebDriver, IWebElement?> selector,
        Func<IWebElement?, object, Task> executor)
    {
        var isolationContext = this.GuidGenerator.Create().ToString();
        try
        {
            //检查是否处于人机验证资源锁定阶段
            if (await this.InterceptorProvider.IsLockedAsync(targetUrl)) return;

            using IWebDriver driver = await this.WebDriverProvider.GetAsync(isolationContext);
            //跳转
            await driver.Navigate().GoToUrlAsync(targetUrl);
            //行为模拟
            await HumanBehavior.ScrollLikeHumanAsync(driver, steps: 2);
            //获取输入框
            driver.FindElement(inputBox).SendKeys(state.ToString());
            //点击按钮
            driver.FindElement(submitBtn).Click();

            //获取跳转后url
            string title = driver.Title;
            string page = driver.PageSource;
            string url = driver.Url;
            string current = driver.CurrentWindowHandle;

            if (string.IsNullOrEmpty(page)) return;
            this.Logger.LogInformation("[ACI]窗口句柄：{Current}，关键字：{Title}，地址：{Url}", current, title, WebUtility.UrlDecode(url));

            //人机验证拦截
            if (await this.InterceptorProvider.LockedAsync(targetUrl, url)) return;

            await driver.Navigate().GoToUrlAsync(driver.Url);
            WebDriverWait wait = new WebDriverWait(driver, timeout: TimeSpan.FromSeconds(30))
            {
                PollingInterval = TimeSpan.FromSeconds(5),
            };
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(WebDriverTimeoutException), typeof(NotFoundException), typeof(StaleElementReferenceException));

            IWebElement? webElement = wait.Until(selector);
            await executor.Invoke(webElement, state);
        }
        catch (Exception exception)
        {
            this.Logger.LogException(exception);
            await executor.Invoke(null, state);
        }
        finally
        {
            this.WebDriverProvider.DeleteBrowserUserProfileDirectories(isolationContext);
        }
    }
}