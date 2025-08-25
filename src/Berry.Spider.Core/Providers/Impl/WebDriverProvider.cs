using Microsoft.Extensions.Options;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using Uri = System.Uri;

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

    /// <summary>
    /// 获取WebDriver
    /// </summary>
    public async Task<IWebDriver> GetAsync(string isolationContext)
    {
        try
        {
            var options = await this.DriverOptionsProvider.BuildAsync(isolationContext);

            if (this.DriverOptions.LocalOptions.IsEnable)
            {
                var cds = this.CreateChromeDriverService(this.DriverOptions.LocalOptions.LocalAddress);
                var driver = new ChromeDriver(cds, options, TimeSpan.FromSeconds(30));
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
            throw new SpiderBizException($"创建WebDriver失败，{e.ToString()}");
        }

        throw new SpiderBizException("not found webdriver...");
    }

    private ChromeDriverService CreateChromeDriverService(string defatltWebDriverPath)
    {
        // In AzDO, the path to the system chromedriver is in an env var called CHROMEWEBDRIVER
        // We want to use this because it should match the installed browser version
        // If the env var is not set, then we fall back on allowing Selenium Manager to download
        // and use an up-to-date chromedriver
        var chromeDriverPathEnvVar = Environment.GetEnvironmentVariable("CHROMEWEBDRIVER") ?? this.GetInnerWebDriverPath(defatltWebDriverPath);
        if (!string.IsNullOrEmpty(chromeDriverPathEnvVar))
        {
            Console.WriteLine(@$"Using chromedriver at path {chromeDriverPathEnvVar}");
            var cds = ChromeDriverService.CreateDefaultService(chromeDriverPathEnvVar);
            cds.HideCommandPromptWindow = true;
            return cds;
        }
        else
        {
            Console.WriteLine(@$"Using default chromedriver from Selenium Manager");
            var cds = ChromeDriverService.CreateDefaultService();
            cds.HideCommandPromptWindow = true;
            return cds;
        }
    }

    /// <summary>
    /// 删除浏览器用户配置目录
    /// </summary>
    public void DeleteBrowserUserProfileDirectories(string isolationContext)
    {
        var userProfileDirectory = UserProfileDirectory(isolationContext);
        if (!string.IsNullOrEmpty(userProfileDirectory) && Directory.Exists(userProfileDirectory))
        {
            var attemptCount = 0;
            while (true)
            {
                try
                {
                    Directory.Delete(userProfileDirectory, recursive: true);
                    break;
                }
                catch (UnauthorizedAccessException ex)
                {
                    attemptCount++;
                    if (attemptCount < 5)
                    {
                        Console.WriteLine(@$"Failed to delete browser profile directory '{userProfileDirectory}': '{ex}'. Will retry.");
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }
    }

    private string UserProfileDirectory(string context)
    {
        return string.IsNullOrEmpty(context) ? string.Empty : Path.Combine(Path.GetTempPath(), "BrowserFixtureUserProfiles", context);
    }

    private string GetInnerWebDriverPath(string defatltWebDriverPath)
    {
        if (OperatingSystem.IsWindows())
        {
            string windowsDriverPath = Path.Combine("web-driver", "windows");
            if (Directory.Exists(windowsDriverPath))
            {
                return windowsDriverPath;
            }
        }
        else if (OperatingSystem.IsMacOS())
        {
            string macosDriverPath = Path.Combine("web-driver", "macos");
            if (Directory.Exists(macosDriverPath))
            {
                return macosDriverPath;
            }
        }

        return defatltWebDriverPath;
    }
}