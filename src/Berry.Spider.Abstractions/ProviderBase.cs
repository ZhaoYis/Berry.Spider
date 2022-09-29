using Berry.Spider.Core;
using Microsoft.Extensions.Logging;

namespace Berry.Spider.Abstractions;

public abstract class ProviderBase<T>
{
    protected ILogger<T> Logger { get; }
    private readonly BloomFilterHelper<string> _bloomFilterHelper;

    protected ProviderBase(ILogger<T> logger)
    {
        this.Logger = logger;
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
            _bloomFilterHelper.Add(keyword);

            //TODO：二次重复性校验

            //执行回调函数
            await checkSuccessCallback.Invoke();
        }
    }
}