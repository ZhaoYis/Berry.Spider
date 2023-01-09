namespace Berry.Spider.Core;

public static class ListExtensions
{
    /// <summary>
    /// 构造出统一风格的主内容文本
    /// </summary>
    /// <returns></returns>
    public static string BuildMainContent(this List<string> items,
        IStringBuilderObjectPoolProvider stringBuilderObjectPoolProvider, List<string>? subTitleList = null)
    {
        if (items.Count == 0) return string.Empty;

        items = items.Where(c => !string.IsNullOrWhiteSpace(c)).ToList();

        string mainContent = stringBuilderObjectPoolProvider.Invoke(builder =>
        {
            if (subTitleList == null || subTitleList.Count == 0)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    string item = items[i];
                    builder.AppendFormat("<p>{0}、{1}</p>", i + 1, item);
                }
            }
            else
            {
                if (items.Count < 25 || items.Count / 2 < 13)
                {
                    //加一个小标题
                    builder.AppendFormat("<p><strong>{0}</strong></p>", subTitleList.First());

                    for (int i = 0; i < items.Count; i++)
                    {
                        string item = items[i];
                        builder.AppendFormat("<p>{0}、{1}</p>", i + 1, item);
                    }
                }
                else
                {
                    int _currentIndex = 0;
                    int _pageSize = 25;
                    int _totalPage = (items.Count / _pageSize) + (items.Count % _pageSize > 0 ? 1 : 0);
                    for (int i = 0; i < _totalPage; i++)
                    {
                        List<string> current = items.Skip(_currentIndex * _pageSize).Take(_pageSize).ToList();
                        if (current.Count == 0) break;

                        if (current.Count == _pageSize || current.Count < current.Count / 2)
                        {
                            string subTitle = i + 1 > subTitleList.Count
                                ? subTitleList.OrderBy(c => Guid.NewGuid()).First()
                                : subTitleList[i];
                            //加一个小标题
                            builder.AppendFormat("<p><strong>{0}</strong></p>", subTitle);

                            for (int j = 0; j < current.Count; j++)
                            {
                                string item = current[j];
                                builder.AppendFormat("<p>{0}、{1}</p>", j + 1, item);
                            }

                            _currentIndex++;
                        }
                        else
                        {
                            for (int k = 0; k < current.Count; k++)
                            {
                                if (k + 1 == current.Count / 2)
                                {
                                    string subTitle = i + 1 > subTitleList.Count
                                        ? subTitleList.OrderBy(c => Guid.NewGuid()).First()
                                        : subTitleList[i];
                                    //加一个小标题
                                    builder.AppendFormat("<p><strong>{0}</strong></p>", subTitle);
                                }

                                string item = current[k];
                                builder.AppendFormat("<p>{0}、{1}</p>", k + 1, item);
                            }

                            _currentIndex++;
                        }
                    }
                }
            }
        });
        return mainContent;
    }

    /// <summary>
    /// 构造出统一风格的主内容文本（文中插入一张图片）
    /// </summary>
    /// <returns></returns>
    public static string BuildMainContent(this List<string> items, IImageResourceProvider provider,
        IStringBuilderObjectPoolProvider stringBuilderObjectPoolProvider, List<string>? subTitleList = null)
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