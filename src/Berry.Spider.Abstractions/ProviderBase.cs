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
            //添加到bloom过滤器
            _bloomFilterHelper.Add(keyword);
            //二次重复性校验
            bool checkSucc = await this.DuplicateCheckAsync(keyword);
            if (checkSucc)
            {
                //执行回调函数
                await checkSuccessCallback.Invoke();
            }
        }
    }

    /// <summary>
    /// 二次重复性校验
    /// </summary>
    /// <returns></returns>
    protected virtual Task<bool> DuplicateCheckAsync(string keyword)
    {
        return Task.FromResult(true);
    }
}