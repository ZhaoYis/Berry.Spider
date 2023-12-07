using OpenQA.Selenium;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.Core;

public interface IWebElementLoadProvider : ISingletonDependency
{
    Task InvokeAsync(string targetUrl,
        object state,
        Func<IWebDriver, IWebElement?> selector,
        Func<IWebElement?, object, Task> executor);

    Task BatchInvokeAsync(IDictionary<string, string> keywordList,
        Func<IWebDriver, IWebElement?> selector,
        Func<IWebElement?, object, Task> executor);

    Task<T?> InvokeAndReturnAsync<T>(string targetUrl,
        object state,
        Func<IWebDriver, IWebElement?> selector,
        Func<IWebElement?, object, Task<T>> executor);

    Task<string> AutoClickAsync(string targetUrl,
        object state,
        By inputBox,
        By submitBtn);

    Task AutoClickAndInvokeAsync(string targetUrl,
        object state,
        By inputBox,
        By submitBtn,
        Func<IWebDriver, IWebElement?> selector,
        Func<IWebElement?, object, Task> executor);
}