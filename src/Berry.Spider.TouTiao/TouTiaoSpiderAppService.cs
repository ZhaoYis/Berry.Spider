using Berry.Spider.Domain.Shared;
using Volo.Abp.Application.Services;

namespace Berry.Spider.TouTiao;

/// <summary>
/// 今日头条爬虫
/// </summary>
public class TouTiaoSpiderAppService : ApplicationService, ITouTiaoSpiderAppService
{
    /// <summary>
    /// 执行爬虫
    /// </summary>
    public async Task ExecuteAsync<T>(T request) where T : ISpiderRequest
    {
        ITouTiaoSpiderProvider? provider = request.SourceFrom switch
        {
            SpiderSourceFrom.TouTiao_Question => this.LazyServiceProvider.LazyGetRequiredService<TouTiaoSpider4QuestionProvider>(),
            SpiderSourceFrom.TouTiao_Information => this.LazyServiceProvider.LazyGetRequiredService<TouTiaoSpider4InformationProvider>(),
            _ => throw new NotImplementedException()
        };

        await provider.ExecuteAsync(request);
    }
}