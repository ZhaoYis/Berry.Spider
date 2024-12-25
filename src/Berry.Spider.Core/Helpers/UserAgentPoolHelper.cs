using System.Text.Json;

namespace Berry.Spider.Core;

/// <summary>
/// User-Agent池。
/// https://useragentstring.com/pages/useragentstring.php?typ=Browser
/// </summary>
public static class UserAgentPoolHelper
{
    private static readonly List<UserAgentMeData>? UserAgentList;

    static UserAgentPoolHelper()
    {
        UserAgentList = JsonSerializer.Deserialize<List<UserAgentMeData>>(DefaultUserAgent.JsonString);
    }

    /// <summary>
    /// 随机从User-Agent池中获取一个User-Agent
    /// </summary>
    /// <returns></returns>
    public static string RandomGetOne()
    {
        if (UserAgentList is { Count: > 0 })
        {
            return UserAgentList.OrderBy(u => Guid.CreateVersion7()).First().ua;
        }

        return "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.0.0 Safari/537.36";
    }

    private class UserAgentMeData
    {
        public string ua { get; set; }

        public decimal pct { get; set; }
    }
}