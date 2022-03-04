using System.Text;

namespace Berry.Spider.Core;

public static class ListExtensions
{
    /// <summary>
    /// 构造出统一风格的主内容文本
    /// </summary>
    /// <returns></returns>
    public static string? BuildMainContent(this List<string> items)
    {
        if (items.Count == 0) return default;

        StringBuilder builder = new StringBuilder();
        int index = 0;

        foreach (string item in items)
        {
            index++;
            builder.AppendFormat("<p>{0}、{1}</p>", index, item);
        }

        return builder.ToString();
    }
}