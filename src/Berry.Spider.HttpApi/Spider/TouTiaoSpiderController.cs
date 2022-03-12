using Berry.Spider.TouTiao;
using Microsoft.AspNetCore.Mvc;

namespace Berry.Spider;

/// <summary>
/// 今日头条爬虫服务
/// </summary>
[Route("api/services/spider/tou-tiao")]
public class TouTiaoSpiderController : SpiderControllerBase
{
    private ITouTiaoSpiderAppService TouTiaoSpiderAppService { get; }

    public TouTiaoSpiderController(ITouTiaoSpiderAppService service)
    {
        this.TouTiaoSpiderAppService = service;
    }

    /// <summary>
    /// 将待爬取信息PUSH到消息队列中
    /// </summary>
    [HttpPost, Route("push")]
    public async Task PushAsync([FromBody] TouTiaoSpiderPushEto push)
    {
        //直接发布事件到MQ，交由Berry.Spider.Consumers消费
        await this.TouTiaoSpiderAppService.PushAsync(push);
    }

    /// <summary>
    /// 将待爬取信息PUSH到消息队列中
    /// </summary>
    [HttpPost, Route("push-from-file")]
    public async Task PushAsync(TouTiaoSpiderPushFromFile push)
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
                string[] rows = await System.IO.File.ReadAllLinesAsync(filePath);
                if (rows.Length > 0)
                {
                    TouTiaoSpiderPushEto eto = new TouTiaoSpiderPushEto();
                    eto.SourceFrom = push.SourceFrom;
                    eto.Keywords.AddRange(rows);

                    await this.TouTiaoSpiderAppService.PushAsync(eto);
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