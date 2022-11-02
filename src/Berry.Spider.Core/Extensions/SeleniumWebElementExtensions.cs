using OpenQA.Selenium;
using System.Collections.ObjectModel;

namespace Berry.Spider.Core;

public static class SeleniumWebElementExtensions
{
    public static IWebElement? TryFindElement(this IWebElement element, By by)
    {
        try
        {
            var e = element.FindElement(by);
            return e;
        }
        catch (Exception e)
        {
            //ignore...
            return default;
        }
    }

    public static ReadOnlyCollection<IWebElement>? TryFindElements(this IWebElement element, By by)
    {
        try
        {
            var e = element.FindElements(by);
            return e;
        }
        catch (Exception e)
        {
            //ignore...
            return default;
        }
    }
}