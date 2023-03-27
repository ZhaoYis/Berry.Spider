using Berry.Spider.Core;
using Microsoft.Extensions.Logging;

namespace Berry.Spider.Abstractions;

public abstract class ProviderBase<T>
{
    private static readonly BloomFilterHelper<string> BloomFilterHelper = new(999999);

    protected ILogger<T> Logger { get; }

    protected ProviderBase(ILogger<T> logger)
    {
        this.Logger = logger;
    }

    /// <summary>
    /// 对待采集关键字逻辑校验
    /// </summary>
    protected async Task CheckAsync(string keyword, SpiderSourceFrom from, Func<Task> checkSuccessCallback, bool bloomCheck = false, bool duplicateCheck = false)
    {
        if (bloomCheck)
        {
            if (BloomFilterHelper.Contains(keyword))
            {
                return;
            }
            else
            {
                //添加到bloom过滤器
                BloomFilterHelper.Add(keyword);

                //二次重复性校验
                if (duplicateCheck)
                {
                    bool checkSucc = await this.DuplicateCheckAsync(keyword, from);
                    if (checkSucc)
                    {
                        //执行回调函数
                        await checkSuccessCallback.Invoke();
                    }
                }
                else
                {
                    //执行回调函数
                    await checkSuccessCallback.Invoke();
                }
            }
        }
        else
        {
            if (duplicateCheck)
            {
                //二次重复性校验
                bool checkSucc = await this.DuplicateCheckAsync(keyword, from);
                if (checkSucc)
                {
                    //执行回调函数
                    await checkSuccessCallback.Invoke();
                }
            }
        }
    }

    /// <summary>
    /// Bloom过滤器重复性校验
    /// </summary>
    protected async Task BloomCheckAsync(string keyword, SpiderSourceFrom from, Func<Task> checkSuccessCallback)
    {
        if (BloomFilterHelper.Contains(keyword))
        {
            return;
        }
        else
        {
            //添加到bloom过滤器
            BloomFilterHelper.Add(keyword);
            //二次重复性校验
            bool checkSucc = await this.DuplicateCheckAsync(keyword, from);
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
    protected virtual Task<bool> DuplicateCheckAsync(string keyword, SpiderSourceFrom from)
    {
        return Task.FromResult(true);
    }
}