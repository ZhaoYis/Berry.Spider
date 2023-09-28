using Volo.Abp.DependencyInjection;

namespace Berry.Spider.Core;

public interface ISeleniumProxyProvider : ISingletonDependency
{
    Task<OpenQA.Selenium.Proxy?> GetProxyAsync();
}