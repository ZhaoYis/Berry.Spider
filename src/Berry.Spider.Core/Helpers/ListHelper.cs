namespace Berry.Spider.Core;

public class ListHelper
{
    private readonly List<string> _items;
    private readonly int _pageSize;
    private readonly int _totalPage;
    private int _pageIndex = 1;

    public ListHelper(List<string> items, int pageSize)
    {
        this._items = items;
        this._pageSize = pageSize;
        this._totalPage = (int)Math.Round(items.Count * 1.0 / pageSize);
    }

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
}