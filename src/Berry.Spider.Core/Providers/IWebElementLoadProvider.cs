using OpenQA.Selenium;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.Core;

public interface IWebElementLoadProvider : ISingletonDependency
{
    Task InvokeAsync(string targetUrl,
        string keyword,
        Func<IWebDriver, IWebElement?> selector,
        Func<IWebElement?, string, Task> executor);

    Task BatchInvokeAsync(IDictionary<string, string> keywordList,
        Func<IWebDriver, IWebElement?> selector,
        Func<IWebElement?, string, Task> executor);

    Task<T?> InvokeAndReturnAsync<T>(string targetUrl,
        string keyword,
        Func<IWebDriver, IWebElement?> selector,
        Func<IWebElement?, string, Task<T>> executor);

    Task<string> AutoClickAsync(string targetUrl,
        string keyword,
        By inputBox,
        By submitBtn);

    Task AutoClickAndInvokeAsync(string targetUrl,
        string keyword,
        By inputBox,
        By submitBtn,
        Func<IWebDriver, IWebElement?> selector,
        Func<IWebElement?, string, Task> executor);
}