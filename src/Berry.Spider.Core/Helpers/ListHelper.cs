namespace Berry.Spider.Core;

public class ListHelper
{
    private readonly Random _random = new Random(Guid.NewGuid().GetHashCode());
    private readonly List<string> _items;
    private readonly int _totalPage;
    private int _pageSize;
    private int _pageIndex = 1;
    private int _totalCount = 0;

    public ListHelper(List<string> items)
    {
        this._items = items;
    }

    public ListHelper(List<string> items, int pageSize) : this(items)
    {
        this._pageSize = pageSize;
        this._totalPage = (int)Math.Round(items.Count * 1.0 / pageSize);
    }

    /// <summary>
    /// 每页获取固定记录数
    /// </summary>
    /// <returns></returns>
    public List<string> GetList()
    {
        if (this._pageIndex <= this._totalPage)
        {
            List<string> result = this._items.Skip((this._pageIndex - 1) * this._pageSize).Take(this._pageSize)
                .ToList();

            this._pageIndex++;
            return result;
        }
        else
        {
            return new List<string>();
        }
    }

    /// <summary>
    /// 每页获取随机记录数
    /// </summary>
    /// <returns></returns>
    public List<string> GetList(int min, int max)
    {
        List<string> result;
        if (_totalCount <= _items.Count)
        {
            _pageSize = this.GetRandomPageSize(min, max);
            result = _items.Skip(_totalCount).Take(_pageSize).ToList();
            _totalCount += _pageSize;
        }
        else
        {
            result = _items.Skip(_totalCount - _pageSize).Take(_items.Count - _totalCount).ToList();
        }

        if (result.Count < min) return new List<string>();
        return result;
    }

    private int GetRandomPageSize(int min, int max)
    {
        int pageSize = _random.Next(min, max);
        return pageSize;
    }
}