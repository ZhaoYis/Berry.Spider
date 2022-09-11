namespace Berry.Spider.Proxy.QgNet;

public class QgNetHttpProxy : IHttpProxy
{
    /// <summary>
    /// 获取代理地址，格式：<HOST:PORT>
    /// </summary>
    /// <returns></returns>
    public Task<string> GetProxyUriAsync()
    {
        //TODO:后台自动任务拉取代理IP
        throw new NotImplementedException();
    }
}