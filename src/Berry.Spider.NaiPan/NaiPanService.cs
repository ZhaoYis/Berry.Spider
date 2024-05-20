using Berry.Spider.NaiPan.weiyuanchuang;

namespace Berry.Spider.NaiPan;

public class NaiPanService : INaiPanService
{
    /// <summary>
    /// 生成伪原创内容
    /// </summary>
    /// <returns></returns>
    public async Task<string> GenerateAsync(string content)
    {
        weiyuanchuangPortTypeClient client = new weiyuanchuangPortTypeClient(weiyuanchuangPortTypeClient.EndpointConfiguration.weiyuanchuangHttpSoap12Endpoint);

        weiyuanchuangRequest request = new weiyuanchuangRequest("", "", content);
        weiyuanchuangResponse response = await client.weiyuanchuangAsync(request);
        if (response is not null)
        {
            return response.@return;
        }

        return string.Empty;
    }
}