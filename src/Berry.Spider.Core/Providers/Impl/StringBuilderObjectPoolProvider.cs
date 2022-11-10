using Microsoft.Extensions.ObjectPool;
using System.Text;

namespace Berry.Spider.Core;

/// <summary>
/// StringBuilder池化
/// </summary>
public class StringBuilderObjectPoolProvider : IStringBuilderObjectPoolProvider
{
    /**
     * https://www.cnblogs.com/InCerry/p/recycle_stringbuilder.html
     */

    private ObjectPoolProvider ObjectPoolProvider { get; }

    public StringBuilderObjectPoolProvider(ObjectPoolProvider provider)
    {
        this.ObjectPoolProvider = provider;
    }

    public string Invoke(Action<StringBuilder> build, int initialCapacity = 256, int maximumRetainedCapacity = 8192)
    {
        var pool = this.ObjectPoolProvider.CreateStringBuilderPool(initialCapacity, maximumRetainedCapacity);

        var builder = pool.Get();
        try
        {
            build.Invoke(builder);
            return builder.ToString();
        }
        finally
        {
            pool.Return(builder);
        }
    }
}