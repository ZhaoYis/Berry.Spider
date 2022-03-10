using Berry.Spider.Domain.Shared;
using Volo.Abp.Application.Services;

namespace Berry.Spider.Baidu;

/// <summary>
/// 百度爬虫
/// </summary>
public class BaiduSpiderAppService : ApplicationService, IBaiduSpiderAppService
{
    /// <summary>
    /// 执行爬虫
    /// </summary>
    public async Task ExecuteAsync<T>(T request) where T : ISpiderRequest
    {
        IBaiduSpiderProvider? provider = request.SourceFrom switch
        {
            SpiderSourceFrom.Baidu_RelatedSearch => this.LazyServiceProvider.LazyGetRequiredService<BaiduSpider4RelatedSearchProvider>(),
            _ => throw new NotImplementedException()
        };

        await provider.ExecuteAsync(request);
    }
}