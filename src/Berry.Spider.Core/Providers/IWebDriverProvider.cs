using OpenQA.Selenium;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.Core;

public interface IWebDriverProvider : ITransientDependency
{
    Task<IWebDriver> GetAsync();
}