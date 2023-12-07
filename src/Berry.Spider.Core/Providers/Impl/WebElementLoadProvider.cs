using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

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
        object state,
        Func<IWebDriver, IWebElement?> selector,
        Func<IWebElement?, object, Task> executor)
    {
        try
        {
            //检查是否处于人机验证资源锁定阶段
            if (await this.InterceptorProvider.IsLockedAsync(targetUrl)) return;

            using IWebDriver driver = await this.WebDriverProvider.GetAsync();
            //跳转
            driver.Navigate().GoToUrl(targetUrl);
            string title = driver.Title;
            string page = driver.PageSource;
            string url = driver.Url;
            string current = driver.CurrentWindowHandle;

            if (string.IsNullOrEmpty(title)) return;
            this.Logger.LogInformation("[V]窗口句柄：{0}，关键字：{1}，地址：{2}", current, title, url);

            //人机验证拦截
            if (await this.InterceptorProvider.LockedAsync(targetUrl, url)) return;

            WebDriverWait wait = new WebDriverWait(driver, timeout: TimeSpan.FromSeconds(30))
            {
                PollingInterval = TimeSpan.FromSeconds(5),
            };
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(WebDriverTimeoutException), typeof(NotFoundException));
            IWebElement? webElement = wait.Until(selector);
            await executor.Invoke(webElement, state);
        }
        catch (Exception exception)
        {
            this.Logger.LogException(exception);
            await executor.Invoke(null, state);
        }
    }

    public async Task BatchInvokeAsync(IDictionary<string, string> keywordList,
        Func<IWebDriver, IWebElement?> selector,
        Func<IWebElement?, object, Task> executor)
    {
        try
        {
            using IWebDriver driver = await this.WebDriverProvider.GetAsync();
            foreach (KeyValuePair<string, string> pair in keywordList)
            {
                try
                {
                    string keyword = pair.Key;
                    string targetUrl = pair.Value;

                    //检查是否处于人机验证资源锁定阶段
                    if (await this.InterceptorProvider.IsLockedAsync(targetUrl)) return;

                    //跳转
                    driver.Navigate().GoToUrl(targetUrl);
                    string title = driver.Title;
                    string page = driver.PageSource;
                    string url = driver.Url;
                    string current = driver.CurrentWindowHandle;

                    if (string.IsNullOrEmpty(title)) return;
                    this.Logger.LogInformation("[BV]窗口句柄：{0}，关键字：{1}，地址：{2}", current, title, url);

                    //人机验证拦截
                    if (await this.InterceptorProvider.LockedAsync(targetUrl, url)) return;

                    WebDriverWait wait = new WebDriverWait(driver, timeout: TimeSpan.FromSeconds(30))
                    {
                        PollingInterval = TimeSpan.FromSeconds(5),
                    };
                    wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(WebDriverTimeoutException), typeof(NotFoundException));
                    IWebElement? webElement = wait.Until(selector);
                    await executor.Invoke(webElement, keyword);
                }
                finally
                {
                    await Task.Delay(20);
                }
            }
        }
        catch (Exception exception)
        {
            this.Logger.LogException(exception);
            await executor.Invoke(null, "");
        }
    }

    public async Task<T?> InvokeAndReturnAsync<T>(string targetUrl,
        object state,
        Func<IWebDriver, IWebElement?> selector,
        Func<IWebElement?, object, Task<T>> executor)
    {
        try
        {
            //检查是否处于人机验证资源锁定阶段
            if (await this.InterceptorProvider.IsLockedAsync(targetUrl)) return default(T);

            using IWebDriver driver = await this.WebDriverProvider.GetAsync();
            //跳转
            driver.Navigate().GoToUrl(targetUrl);
            string title = driver.Title;
            string page = driver.PageSource;
            string url = driver.Url;
            string current = driver.CurrentWindowHandle;

            if (string.IsNullOrEmpty(title)) return default;
            this.Logger.LogInformation("[T]窗口句柄：{0}，关键字：{1}，地址：{2}", current, title, url);

            //人机验证拦截
            if (await this.InterceptorProvider.LockedAsync(targetUrl, url)) return await Task.FromResult(default(T));

            WebDriverWait wait = new WebDriverWait(driver, timeout: TimeSpan.FromSeconds(30))
            {
                PollingInterval = TimeSpan.FromSeconds(5),
            };
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(WebDriverTimeoutException), typeof(NotFoundException));
            IWebElement? webElement = wait.Until(selector);
            T result = await executor.Invoke(webElement, state);
            return result;
        }
        catch (Exception exception)
        {
            this.Logger.LogException(exception);
            await executor.Invoke(null, state);
        }

        return default(T);
    }

    public async Task<string> AutoClickAsync(string targetUrl,
        object state,
        By inputBox,
        By submitBtn)
    {
        try
        {
            //检查是否处于人机验证资源锁定阶段
            if (await this.InterceptorProvider.IsLockedAsync(targetUrl)) return string.Empty;

            using IWebDriver driver = await this.WebDriverProvider.GetAsync();
            //跳转
            driver.Navigate().GoToUrl(targetUrl);
            //获取输入框
            driver.FindElement(inputBox).SendKeys(state.ToString());
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

            //人机验证拦截
            if (await this.InterceptorProvider.LockedAsync(targetUrl, url)) return string.Empty;

            return url;
        }
        catch (Exception exception)
        {
            this.Logger.LogException(exception);
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
        try
        {
            //检查是否处于人机验证资源锁定阶段
            if (await this.InterceptorProvider.IsLockedAsync(targetUrl)) return;

            using IWebDriver driver = await this.WebDriverProvider.GetAsync();
            //跳转
            driver.Navigate().GoToUrl(targetUrl);
            //获取输入框
            driver.FindElement(inputBox).SendKeys(state.ToString());
            //点击按钮
            driver.FindElement(submitBtn).Click();

            //获取跳转后url
            string title = driver.Title;
            string url = driver.Url;
            string current = driver.CurrentWindowHandle;

            if (string.IsNullOrEmpty(title)) return;
            this.Logger.LogInformation("[ACI]窗口句柄：{0}，关键字：{1}，地址：{2}", current, title, url);

            //人机验证拦截
            if (await this.InterceptorProvider.LockedAsync(targetUrl, url)) return;

            driver.Navigate().GoToUrl(driver.Url);
            WebDriverWait wait = new WebDriverWait(driver, timeout: TimeSpan.FromSeconds(30))
            {
                PollingInterval = TimeSpan.FromSeconds(5),
            };
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(WebDriverTimeoutException), typeof(NotFoundException));

            IWebElement? webElement = wait.Until(selector);
            await executor.Invoke(webElement, state);
        }
        catch (Exception exception)
        {
            this.Logger.LogException(exception);
            await executor.Invoke(null, state);
        }
    }
}