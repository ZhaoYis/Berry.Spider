using Berry.Spider.Contracts;
using Berry.Spider.Core.Exceptions;
using Microsoft.Extensions.Options;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

namespace Berry.Spider.Core;

public class WebDriverProvider : IWebDriverProvider
{
    private IOptionsSnapshot<WebDriverOptions> DriverOptions { get; }
    private IDriverOptionsProvider DriverOptionsProvider { get; }

    public WebDriverProvider(IOptionsSnapshot<WebDriverOptions> options, IDriverOptionsProvider optionsProvider)
    {
        this.DriverOptions = options;
        this.DriverOptionsProvider = optionsProvider;
    }

    public async Task<IWebDriver> GetAsync()
    {
        try
        {
            var options = await this.DriverOptionsProvider.BuildAsync();

            if (this.DriverOptions.Value.LocalOptions.IsEnable)
            {
                IWebDriver driver = new ChromeDriver(this.DriverOptions.Value.LocalOptions.LocalAddress, options);
                return driver;
            }
            else if (this.DriverOptions.Value.RemoteOptions.IsEnable)
            {
                IWebDriver driver = new RemoteWebDriver(new Uri(this.DriverOptions.Value.RemoteOptions.RemoteAddress), options);
                return driver;
            }
        }
        catch (Exception e)
        {
            throw new SpiderBizException($"创建WebDriver失败，{e.Message}");
        }

        throw new SpiderBizException("not no found webdriver...");
    }
}