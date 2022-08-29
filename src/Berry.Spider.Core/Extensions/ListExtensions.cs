using System.Text;

namespace Berry.Spider.Core;

public static class ListExtensions
{
    /// <summary>
    /// 构造出统一风格的主内容文本
    /// </summary>
    /// <returns></returns>
    public static string BuildMainContent(this List<string> items)
    {
        if (items.Count == 0) return string.Empty;

        StringBuilder builder = new StringBuilder();
        int index = 0;

        items = items.Where(c => !string.IsNullOrWhiteSpace(c)).ToList();

        foreach (string item in items)
        {
            index++;
            builder.AppendFormat("<p>{0}、{1}</p>", index, item);
        }

        return builder.ToString();
    }

    /// <summary>
    /// 构造出统一风格的主内容文本（文中插入一张图片）
    /// </summary>
    /// <returns></returns>
    public static string BuildMainContent(this List<string> items, IImageResourceProvider provider)
    {
        if (items.Count == 0) return string.Empty;

        StringBuilder builder = new StringBuilder();
        int index = 0;

        items = items.Where(c => !string.IsNullOrWhiteSpace(c)).ToList();

        foreach (string item in items)
        {
            index++;

            if (index == items.Count / 2)
            {
                string imageUrl = provider.TryGet();
                builder.AppendFormat("<p><img src='{0}' alt=''></img></p>", imageUrl);
            }

            builder.AppendFormat("<p>{0}、{1}</p>", index, item);
        }

        return builder.ToString();
    }

    /// <summary>
    /// 随机打乱List集合
    /// </summary>
    /// <returns></returns>
    public static void RandomSort<T>(this List<T> sources)
    {
        Random rd = new Random(new Guid().GetHashCode());
        for (int i = 0; i < sources.Count - 1; i++)
        {
            var index = rd.Next(0, sources.Count - 1);
            if (index != i)
            {
                (sources[i], sources[index]) = (sources[index], sources[i]);
            }
        }
    }
}