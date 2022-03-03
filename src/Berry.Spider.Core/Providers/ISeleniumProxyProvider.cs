using Volo.Abp.DependencyInjection;

namespace Berry.Spider.Core;

public interface ISeleniumProxyProvider : ITransientDependency
{
    Task<OpenQA.Selenium.Proxy?> GetProxyAsync();
}