using Berry.Spider.NaiPan.weiyuanchuang;
using Microsoft.Extensions.Options;

namespace Berry.Spider.NaiPan;

public class NaiPanService : INaiPanService
{
    private NaiPanOptions Options { get; }

    public NaiPanService(IOptionsSnapshot<NaiPanOptions> options)
    {
        this.Options = options.Value;
    }

    /// <summary>
    /// 生成伪原创内容
    /// </summary>
    /// <returns></returns>
    public async Task<string> GenerateAsync(string content)
    {
        if (this.Options is { IsEnabled: true })
        {
            weiyuanchuangPortTypeClient client = new weiyuanchuangPortTypeClient(weiyuanchuangPortTypeClient.EndpointConfiguration.weiyuanchuangHttpSoap12Endpoint);

            weiyuanchuangRequest request = new weiyuanchuangRequest(this.Options.Account, this.Options.Secret, content);
            weiyuanchuangResponse response = await client.weiyuanchuangAsync(request);
            if (response is not null)
            {
                return response.@return;
            }
        }

        return content;
    }
}