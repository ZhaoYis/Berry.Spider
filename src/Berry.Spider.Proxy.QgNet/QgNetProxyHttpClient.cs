using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace Berry.Spider.Proxy.QgNet;

public class QgNetProxyHttpClient
{
    private QgNetProxyOptions Options { get; }
    private HttpClient Client { get; }

    public QgNetProxyHttpClient(IOptionsSnapshot<QgNetProxyOptions> options, HttpClient httpClient)
    {
        this.Options = options.Value;

        this.Client = httpClient;
        this.Client.BaseAddress = new Uri(this.Options.ProxyPoolApiHost);
    }

    /// <summary>
    /// 随机获取一个代理IP
    /// </summary>
    /// <returns></returns>
    public async Task<QgNetProxyResult?> GetOneAsync()
    {
        // 参数名	    是否必选	    类型	        描述
        // Key	        是	        String	    申请的Key值
        // Num	        否	        Integer	    申请的数量;默认1个
        // KeepAlive	否	        Integer	    生存周期;默认动态独享24小时,动态共享默认购买的套餐存活周期时长
        // AreaId	    否	        Integer	    区域ID;默认随机
        // ISP	        否	        Integer	    运营商ID;默认随机
        // Detail	    否	        Integer	    详情0(关闭) 1(开启) ，默认为 0
        // Distinct	    否	        Integer	    去重0(关闭) 1(开启) ，默认为 0

        try
        {
            var result =
                await this.Client.GetFromJsonAsync<QgNetResult<List<QgNetProxyResult>>>(
                    $"/allocate?Key={this.Options.AuthKey}&Detail=1&Distinct=1&Num=1");
            if (result is { IsSuccess: true })
            {
                List<QgNetProxyResult> data = result.Data;
                return data.First();
            }
        }
        catch (Exception e)
        {
            return default;
        }

        return default;
    }

    /// <summary>
    /// 查询可用通道数
    /// </summary>
    /// <returns></returns>
    public async Task<QgNetProxyQuotaResult?> GetQuotaResultAsync()
    {
        try
        {
            var result =
                await this.Client.GetFromJsonAsync<QgNetProxyQuotaResult>(
                    $"/info/quota?Key={this.Options.AuthKey}");
            if (result is { IsSuccess: true })
            {
                return result;
            }
        }
        catch (Exception e)
        {
            return default;
        }

        return default;
    }
}