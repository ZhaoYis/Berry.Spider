using OpenQA.Selenium.Chrome;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.TouTiao;

public interface IDriverOptionsProvider : ITransientDependency
{
    Task<ChromeOptions> BuildAsync(bool isUsedProxy = true);
}