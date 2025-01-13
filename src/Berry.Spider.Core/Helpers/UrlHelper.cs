using System.Net;
using System.Text.RegularExpressions;

namespace Berry.Spider.Core;

public static partial class UrlHelper
{
    /// <summary>
    /// 提取url的主机地址
    /// </summary>
    /// <returns></returns>
    public static string GetHostString(string url)
    {
        try
        {
            Regex regex = new Regex(@"^([a-zA-Z0-9]+:\/\/)?([0-9a-zA-Z-.]+)(:\d+)?(\/.*)?$");
            if (regex.Match(url).Success)
            {
                return regex.Match(url).Groups[2].Value;
            }
        }
        catch (Exception)
        {
            // ignored
        }

        return url.ToMd5();
    }

    /// <summary>
    /// 是否符合url规范
    /// </summary>
    /// <returns></returns>
    public static bool IsUrl(string source)
    {
        Regex regex = new Regex(@"^(http?|https?|ftp):\/\/[^\s\/$.?#].[^\s]*$");
        return regex.Match(source).Success;
    }

    /// <summary>
    /// 解码
    /// </summary>
    /// <returns></returns>
    public static string UrlDecode(string url)
    {
        return WebUtility.UrlDecode(url);
    }
}