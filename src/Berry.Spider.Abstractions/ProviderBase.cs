using Berry.Spider.Core;

namespace Berry.Spider.Abstractions;

public abstract class ProviderBase
{
    private readonly BloomFilterHelper<string> _bloomFilterHelper;

    protected ProviderBase()
    {
        this._bloomFilterHelper = new BloomFilterHelper<string>(999999);
    }

    /// <summary>
    /// 对待采集关键字进行一次重复性校验
    /// </summary>
    protected async Task BloomCheckAsync(string keyword, Func<Task> checkSuccessCallback)
    {
        if (_bloomFilterHelper.Contains(keyword))
        {
            return;
        }
        else
        {
            //执行回调函数
            await checkSuccessCallback.Invoke();

            _bloomFilterHelper.Add(keyword);
        }
    }
}