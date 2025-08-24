using System.Collections.Concurrent;
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

    private readonly ConcurrentDictionary<string, ChromeDriverService> _driverServices = new();
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
                if (_browsers.TryGetValue(isolationContext, out var existingDriver))
                {
                    return existingDriver;
                }

                lock (_browsers)
                {
                    return _browsers.GetOrAdd(isolationContext, () =>
                    {
                        var cds = this.CreateChromeDriverService(this.DriverOptions.LocalOptions.LocalAddress);
                        cds.HideCommandPromptWindow = true;
                        var driver = new ChromeDriver(cds, options, TimeSpan.FromSeconds(30));
                        _driverServices.TryAdd(isolationContext, cds);
                        return driver;
                    });
                }
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
        // 并行处理资源释放
        var disposalTasks = _browsers.Select(kvp => Task.Run(() =>
        {
            try
            {
                kvp.Value?.Quit();
                kvp.Value?.Dispose();
                if (_driverServices.TryRemove(kvp.Key, out var service))
                {
                    service.Dispose();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"释放WebDriver资源失败：{ex.Message}");
            }
        }));
        await Task.WhenAll(disposalTasks);
        //删除浏览器用户配置目录
        await DeleteBrowserUserProfileDirectoriesAsync();
    }

    /// <summary>
    /// 删除浏览器用户配置目录
    /// </summary>
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
                            await Task.Delay(500);
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
            }
        }

        //清空缓存的对象
        _browsers.Clear();
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