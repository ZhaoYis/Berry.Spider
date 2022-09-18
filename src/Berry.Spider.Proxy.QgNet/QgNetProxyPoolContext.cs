namespace Berry.Spider.Proxy.QgNet;

internal static class QgNetProxyPoolContext
{
    private static readonly AsyncLocal<QgNetProxyResult?> AsyncLocalContext;

    static QgNetProxyPoolContext()
    {
        AsyncLocalContext = new AsyncLocal<QgNetProxyResult?>();
    }

    public static void Set(QgNetProxyResult? result)
    {
        AsyncLocalContext.Value = result;
    }

    public static QgNetProxyResult? Get()
    {
        QgNetProxyResult? result = AsyncLocalContext.Value;
        //检查当前IP是否有效
        if (result is {IsInvalid: true})
        {
            return result;
        }

        return default;
    }
}