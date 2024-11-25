using Berry.Spider.Common;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Berry.Spider.Core;
using Volo.Abp;

namespace Berry.Spider;

/// <summary>
/// 公共服务
/// </summary>
[Area(AppGlobalConstants.ModelName)]
[Route("api/services/common")]
[RemoteService(Name = AppGlobalConstants.RemoteServiceName)]
public class CommonController : SpiderControllerBase
{
    /// <summary>
    /// 内容去重
    /// </summary>
    [HttpPost, Route("de-duplication"), DisableRequestSizeLimit]
    public async Task<FileContentResult> TxtDeDuplicationAsync(DeDuplicationTxtFile push)
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
                    byte[] buffer = Encoding.UTF8.GetBytes(string.Join("\n", rows));
                    await using (var stream = new MemoryStream())
                    {
                        await stream.WriteAsync(buffer, 0, buffer.Length);

                        FileContentResult result = new FileContentResult(stream.GetBuffer(), "text/plain")
                        {
                            FileDownloadName = Guid.CreateVersion7().ToString("N") + ".txt"
                        };

                        return result;
                    }
                }
            }

            throw new SpiderBizException("文件生成失败！");
        }
        catch (Exception)
        {
            //ignore..
            throw new SpiderBizException("文件生成失败！");
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