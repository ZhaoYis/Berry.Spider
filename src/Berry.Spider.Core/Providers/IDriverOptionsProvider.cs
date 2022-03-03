using OpenQA.Selenium.Chrome;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.Core;

public interface IDriverOptionsProvider : ITransientDependency
{
    Task<ChromeOptions> BuildAsync(bool isUsedProxy = true);
}