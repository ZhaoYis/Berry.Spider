using System.Text;

namespace Berry.Spider.Core;

public static class ListExtensions
{
    /// <summary>
    /// 构造出统一风格的主内容文本
    /// </summary>
    /// <returns></returns>
    public static string BuildMainContent(this List<string> items, IStringBuilderObjectPoolProvider stringBuilderObjectPoolProvider)
    {
        if (items.Count == 0) return string.Empty;

        items = items.Where(c => !string.IsNullOrWhiteSpace(c)).ToList();

        string mainContent = stringBuilderObjectPoolProvider.Invoke(builder =>
        {
            for (int i = 0; i < items.Count; i++)
            {
                string item = items[i];
                builder.AppendFormat("<p>{0}、{1}</p>", i + 1, item);
            }
        });
        return mainContent;
    }

    /// <summary>
    /// 构造出统一风格的主内容文本（文中插入一张图片）
    /// </summary>
    /// <returns></returns>
    public static string BuildMainContent(this List<string> items, IImageResourceProvider provider, IStringBuilderObjectPoolProvider stringBuilderObjectPoolProvider)
    {
        if (items.Count == 0) return string.Empty;

        items = items.Where(c => !string.IsNullOrWhiteSpace(c)).ToList();

        string mainContent = stringBuilderObjectPoolProvider.Invoke(builder =>
        {
            for (int i = 0; i < items.Count; i++)
            {
                string item = items[i];

                if (i == items.Count / 2)
                {
                    string imageUrl = provider.TryGet();
                    builder.AppendFormat("<p><img src='{0}' alt=''></img></p>", imageUrl);
                }

                builder.AppendFormat("<p>{0}、{1}</p>", i + 1, item);
            }
        });
        return mainContent;
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