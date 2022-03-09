using Berry.Spider.Contracts;
using Berry.Spider.Domain.Shared;
using Volo.Abp.Application.Services;

namespace Berry.Spider.TouTiao;

/// <summary>
/// 今日头条爬虫
/// </summary>
public class TouTiaoSpiderAppService : ApplicationService, ITouTiaoSpiderAppService
{
    private IEnumerable<ITouTiaoSpiderProvider> TiaoSpiderProviders { get; }

    public TouTiaoSpiderAppService(IEnumerable<ITouTiaoSpiderProvider> spiderProviders)
    {
        this.TiaoSpiderProviders = spiderProviders;
    }

    /// <summary>
    /// 执行爬虫
    /// </summary>
    public async Task ExecuteAsync<T>(T request) where T : ISpiderRequest
    {
        foreach (ITouTiaoSpiderProvider provider in this.TiaoSpiderProviders)
        {
            if (provider is TouTiaoSpider4QuestionProvider &&
                request.SourceFrom == SpiderSourceFrom.TouTiao_Question)
            {
                await provider.ExecuteAsync(request);
            }
            else if (provider is TouTiaoSpider4InformationProvider &&
                     request.SourceFrom == SpiderSourceFrom.TouTiao_Information)
            {
                await provider.ExecuteAsync(request);
            }
        }
    }
}