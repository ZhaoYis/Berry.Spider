using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace Berry.Spider.Core;

public static class HumanBehavior
{
    private static readonly Random R = new Random();

    private static async Task PauseAsync(int minMs = 120, int maxMs = 350)
    {
        await Task.Delay(R.Next(minMs, maxMs));
    }

    /// <summary>
    /// 模拟滚动
    /// </summary>
    /// <param name="driver"></param>
    /// <param name="steps"></param>
    public static async Task ScrollLikeHumanAsync(IWebDriver driver, int steps = 8)
    {
        var js = (IJavaScriptExecutor)driver;
        for (int i = 0; i < steps; i++)
        {
            var delta = R.Next(250, 900);
            js.ExecuteScript($"window.scrollBy(0,{delta});");
            await PauseAsync(120, 420);
        }

        // 小幅回滚
        js.ExecuteScript($"window.scrollBy(0,{-R.Next(80, 180)});");
        await PauseAsync(150, 350);
    }

    /// <summary>
    /// 模拟鼠标移动
    /// </summary>
    /// <param name="driver"></param>
    /// <param name="el"></param>
    public static async Task MoveAndClickAsync(IWebDriver driver, IWebElement el)
    {
        var size = el.Size;
        var offsetX = R.Next(Math.Max(1, size.Width / 4), Math.Max(2, (size.Width * 3) / 4));
        var offsetY = R.Next(Math.Max(1, size.Height / 4), Math.Max(2, (size.Height * 3) / 4));

        var actions = new Actions(driver);
        actions.MoveToElement(el, offsetX, offsetY)
            .Pause(TimeSpan.FromMilliseconds(R.Next(80, 220)))
            .Click()
            .Perform();

        await PauseAsync(120, 280);
    }

    /// <summary>
    /// 模拟输入节奏
    /// </summary>
    /// <param name="input"></param>
    /// <param name="text"></param>
    /// <param name="minDelay"></param>
    /// <param name="maxDelay"></param>
    public static async Task TypeLikeHumanAsync(IWebElement input, string text,
        int minDelay = 40, int maxDelay = 120)
    {
        foreach (var ch in text)
        {
            input.SendKeys(ch.ToString());
            await Task.Delay(R.Next(minDelay, maxDelay));
            if (R.NextDouble() < 0.04) // 偶发犹豫
                await Task.Delay(R.Next(150, 300));
        }
    }
}