using OpenQA.Selenium;

namespace Berry.Spider.Core;

public class ServerBase : BrowserBase
{
    protected ServerBase(IWebDriverProvider webDriverProvider) : base(webDriverProvider)
    {
    }

    public void Navigate(Uri relativeUrl, bool noReload = false)
    {
        Browser.Navigate(relativeUrl, noReload);
    }

    protected override Task InitializeAsyncCore()
    {
        // Clear logs - we check these during tests in some cases.
        // Make sure each test starts clean.
        ((IJavaScriptExecutor)Browser).ExecuteScript("console.clear()");
        return base.InitializeAsyncCore();
    }
}