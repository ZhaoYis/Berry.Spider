using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using Uri = System.Uri;

namespace Berry.Spider.Core;

public class WebDriverProvider : IWebDriverProvider, IAsyncDisposable
{
    private WebDriverOptions DriverOptions { get; }
    private IDriverOptionsProvider DriverOptionsProvider { get; }

    private readonly ConcurrentDictionary<string, IWebDriver> _browsers = new();

    public WebDriverProvider(IOptionsSnapshot<WebDriverOptions> options, IDriverOptionsProvider optionsProvider)
    {
        this.DriverOptions = options.Value;
        this.DriverOptionsProvider = optionsProvider;
    }

    public async Task<IWebDriver> GetAsync(string isolationContext = "")
    {
        try
        {
            var options = await this.DriverOptionsProvider.BuildAsync(isolationContext);

            if (this.DriverOptions.LocalOptions.IsEnable)
            {
                return _browsers.GetOrAdd(isolationContext, () =>
                {
                    var cds = this.CreateChromeDriverService(this.DriverOptions.LocalOptions.LocalAddress);
                    IWebDriver driver = new ChromeDriver(
                        cds,
                        options,
                        TimeSpan.FromSeconds(30)
                    );

                    return driver;
                });
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
            return ChromeDriverService.CreateDefaultService(chromeDriverPathEnvVar);
        }
        else
        {
            Console.WriteLine(@$"Using default chromedriver from Selenium Manager");
            return ChromeDriverService.CreateDefaultService();
        }
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or
    /// resetting unmanaged resources asynchronously.</summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    public async ValueTask DisposeAsync()
    {
        var browsers = _browsers.Values;
        foreach (var browser in browsers)
        {
            browser?.Quit();
            browser?.Dispose();
        }

        await DeleteBrowserUserProfileDirectoriesAsync();
    }

    private async Task DeleteBrowserUserProfileDirectoriesAsync()
    {
        foreach (var context in _browsers.Keys)
        {
            var userProfileDirectory = UserProfileDirectory(context);
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
                            await Task.Delay(2000);
                        }
                        else
                        {
                            throw;
                        }
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
            return Path.Combine("web-driver", "windows");
        }
        else if (OperatingSystem.IsMacOS())
        {
            return Path.Combine("web-driver", "macos");
        }

        return defatltWebDriverPath;
    }
}