using OpenQA.Selenium;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.Core;

public interface IWebDriverProvider : ISingletonDependency, IAsyncDisposable
{
    Task<IWebDriver> GetAsync(string isolationContext = "");
}