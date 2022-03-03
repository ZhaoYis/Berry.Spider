using Volo.Abp.DependencyInjection;

namespace Berry.Spider.TouTiao;

public interface ISeleniumProxyProvider : ITransientDependency
{
    Task<OpenQA.Selenium.Proxy?> GetProxyAsync();
}