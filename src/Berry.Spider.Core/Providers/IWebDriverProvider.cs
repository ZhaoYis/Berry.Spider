using OpenQA.Selenium;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.Core;

public interface IWebDriverProvider : ISingletonDependency
{
    /// <summary>
    /// 获取WebDriver
    /// </summary>
    Task<IWebDriver> GetAsync(string isolationContext);

    /// <summary>
    /// 删除浏览器用户配置目录
    /// </summary>
    void DeleteBrowserUserProfileDirectories(string isolationContext);
}