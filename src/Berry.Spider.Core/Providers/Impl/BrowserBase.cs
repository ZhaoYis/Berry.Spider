using System.Runtime.ExceptionServices;
using OpenQA.Selenium;

namespace Berry.Spider.Core;

public class BrowserBase
{
    private ExceptionDispatchInfo _exceptionDispatchInfo;
    private IWebDriver _browser;

    private IWebDriverProvider WebDriverProvider { get; }

    protected BrowserBase(IWebDriverProvider webDriverProvider)
    {
        WebDriverProvider = webDriverProvider;
    }

    protected IWebDriver Browser
    {
        get
        {
            if (_exceptionDispatchInfo != null)
            {
                _exceptionDispatchInfo.Throw();
                throw _exceptionDispatchInfo.SourceException;
            }

            return _browser;
        }
        private set => _browser = value;
    }

    protected virtual async Task InitializeAsync(string isolationContext)
    {
        await InitializeBrowserAsync(isolationContext);
        await InitializeAsyncCore();
    }

    protected virtual Task InitializeAsyncCore()
    {
        return Task.CompletedTask;
    }

    private async Task InitializeBrowserAsync(string isolationContext)
    {
        try
        {
            var browser = await WebDriverProvider.GetAsync(isolationContext);
            Browser = browser;
        }
        catch (Exception ex)
        {
            _exceptionDispatchInfo = ExceptionDispatchInfo.Capture(ex);
            throw;
        }
    }
}