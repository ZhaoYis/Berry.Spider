using OpenQA.Selenium;

namespace Berry.Spider.Core;

public static class WebDriverExtensions
{
    public static void Navigate(this IWebDriver browser, Uri relativeUrl, bool noReload)
    {
        browser.Navigate().GoToUrl("about:blank");
        browser.Navigate().GoToUrl(relativeUrl);
    }
}