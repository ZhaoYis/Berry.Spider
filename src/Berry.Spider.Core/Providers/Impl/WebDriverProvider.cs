using Berry.Spider.Contracts;
using Berry.Spider.Core.Exceptions;
using Microsoft.Extensions.Options;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

namespace Berry.Spider.Core;

public class WebDriverProvider : IWebDriverProvider
{
    private WebDriverOptions DriverOptions { get; }
    private IDriverOptionsProvider DriverOptionsProvider { get; }

    public WebDriverProvider(IOptionsSnapshot<WebDriverOptions> options, IDriverOptionsProvider optionsProvider)
    {
        this.DriverOptions = options.Value;
        this.DriverOptionsProvider = optionsProvider;
    }

    public async Task<IWebDriver> GetAsync()
    {
        try
        {
            var options = await this.DriverOptionsProvider.BuildAsync();

            if (this.DriverOptions.LocalOptions.IsEnable)
            {
                IWebDriver driver = new ChromeDriver(this.DriverOptions.LocalOptions.LocalAddress, options);
                return driver;
            }
            else if (this.DriverOptions.RemoteOptions.IsEnable)
            {
                IWebDriver driver = new RemoteWebDriver(new Uri(this.DriverOptions.RemoteOptions.RemoteAddress), options);
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