using OpenQA.Selenium.Chrome;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.Core;

public interface IDriverOptionsProvider : ISingletonDependency
{
    Task<ChromeOptions> BuildAsync(string context, bool isUsedProxy = true);
}