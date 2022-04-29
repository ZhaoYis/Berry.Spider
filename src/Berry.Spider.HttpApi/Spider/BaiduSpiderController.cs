using Berry.Spider.Baidu;
using Berry.Spider.TouTiao;
using Microsoft.AspNetCore.Mvc;

namespace Berry.Spider;

/// <summary>
/// 百度爬虫服务
/// </summary>
[Route("api/services/spider/baidu")]
public class BaiduSpiderController : SpiderControllerBase
{
    private IBaiduSpiderAppService BaiduSpiderAppService { get; }

    public BaiduSpiderController(IBaiduSpiderAppService service)
    {
        this.BaiduSpiderAppService = service;
    }

    /// <summary>
    /// 将待爬取信息PUSH到消息队列中
    /// </summary>
    [HttpPost, Route("push")]
    public Task PushAsync([FromBody] BaiduSpiderPushEto push)
    {
        //直接发布事件到MQ，交由Berry.Spider.Consumers消费
        return this.BaiduSpiderAppService.PushAsync(push);
    }
    
    /// <summary>
    /// 将待爬取信息PUSH到消息队列中
    /// </summary>
    [HttpPost, Route("push-from-file")]
    public async Task PushAsync(BaiduSpiderPushFromFile push)
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
                        if(string.IsNullOrWhiteSpace(row.Trim())) continue;

                        BaiduSpiderPushEto eto = new BaiduSpiderPushEto
                        {
                            SourceFrom = push.SourceFrom,
                            Keyword = row
                        };

                        await this.BaiduSpiderAppService.PushAsync(eto);
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