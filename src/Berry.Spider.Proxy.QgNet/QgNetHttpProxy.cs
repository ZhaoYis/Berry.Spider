namespace Berry.Spider.Proxy.QgNet;

public class QgNetHttpProxy : IHttpProxy
{
    private QgNetProxyHttpClient QgNetProxyHttpClient { get; }

    public QgNetHttpProxy(QgNetProxyHttpClient httpClient)
    {
        this.QgNetProxyHttpClient = httpClient;
    }

    /// <summary>
    /// 获取代理地址，格式：<HOST:PORT>
    /// </summary>
    /// <returns></returns>
    public async Task<string> GetProxyUriAsync()
    {
        QgNetProxyResult? qgNetProxyResult = QgNetProxyPoolContext.Get();
        if (qgNetProxyResult == null)
        {
            qgNetProxyResult = await this.QgNetProxyHttpClient.GetOneAsync();
            QgNetProxyPoolContext.Set(qgNetProxyResult);
        }

        if (qgNetProxyResult != null) return qgNetProxyResult.Host;

        return string.Empty;
    }
}