using Berry.Spider.Sogou;
using Berry.Spider.Sogou.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Berry.Spider;

/// <summary>
/// 搜狗爬虫服务
/// </summary>
[Route("api/services/spider/sogou")]
public class SogouSpiderController : SpiderControllerBase
{
    private ISogouSpiderAppService SogouSpiderAppService { get; }

    public SogouSpiderController(ISogouSpiderAppService service)
    {
        this.SogouSpiderAppService = service;
    }

    /// <summary>
    /// 将待爬取信息PUSH到消息队列中
    /// </summary>
    [HttpPost, Route("push")]
    public Task PushAsync([FromBody] SogouSpiderPushEto push)
    {
        //直接发布事件到MQ，交由Berry.Spider.Consumers消费
        return this.SogouSpiderAppService.PushAsync(push);
    }

    /// <summary>
    /// 将待爬取信息PUSH到消息队列中
    /// </summary>
    [HttpPost, Route("push-from-file"), DisableRequestSizeLimit]
    public async Task PushAsync(SogouSpiderPushFromFile push)
    {
        var filePath = Path.GetTempFileName();

        try
        {
            await using (var stream = System.IO.File.Create(filePath))
            {
                await push.File.CopyToAsync(stream);
            }

            if (System.IO.File.Exists(filePath))
            {
                List<string> rows = (await System.IO.File.ReadAllLinesAsync(filePath))
                    .Where(c => !string.IsNullOrWhiteSpace(c.Trim()))
                    .Distinct()
                    .ToList();
                if (rows.Count > 0)
                {
                    foreach (string row in rows)
                    {
                        SogouSpiderPushEto eto = new SogouSpiderPushEto
                        {
                            SourceFrom = push.SourceFrom,
                            Keyword = row
                        };

                        await this.SogouSpiderAppService.PushAsync(eto);
                    }
                }
            }
        }
        catch (Exception e)
        {
            //ignore..
        }
        finally
        {
            //删除临时文件
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
    }
}