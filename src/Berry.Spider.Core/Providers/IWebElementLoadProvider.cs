using OpenQA.Selenium;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.Core;

public interface IWebElementLoadProvider : ITransientDependency
{
    Task InvokeAsync(string targetUrl, Func<IWebDriver, IWebElement?> selector, Func<IWebElement?, Task> executor);

    Task<T?> InvokeAsync<T>(string targetUrl, Func<IWebDriver, IWebElement?> selector,
        Func<IWebElement?, Task<T>> executor);

    Task<string> AutoClickAsync(string targetUrl, string keyword, By inputBox, By submitBtn);
}