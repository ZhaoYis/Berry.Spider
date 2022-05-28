using Volo.Abp.Application.Services;

namespace Berry.Spider.Mmonly.Contracts;

public interface IMmonlySpiderAppService: IApplicationService
{
    /// <summary>
    /// 下载https://www.mmonly.cc的图片资源
    /// </summary>
    /// <returns></returns>
    Task DownloadAsync();
}