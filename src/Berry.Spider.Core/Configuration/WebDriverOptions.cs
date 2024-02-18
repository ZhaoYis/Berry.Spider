namespace Berry.Spider.Core;

/// <summary>
/// WebDriver配置选项
/// </summary>
public class WebDriverOptions
{
    /// <summary>
    /// 远程WebDriver
    /// </summary>
    public RemoteWebDriverOptions RemoteOptions { get; set; }

    /// <summary>
    /// 本地WebDriver
    /// </summary>
    public LocalWebDriverOptions LocalOptions { get; set; }
}

/// <summary>
/// 远程WebDriver
/// </summary>
public class RemoteWebDriverOptions
{
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnable { get; set; }

    /// <summary>
    /// 远程WebDriver地址
    /// </summary>
    public string RemoteAddress { get; set; }
}

/// <summary>
/// 本地WebDriver
/// </summary>
public class LocalWebDriverOptions
{
    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnable { get; set; }

    /// <summary>
    /// 本地WebDriver地址
    /// </summary>
    public string LocalAddress { get; set; }
}