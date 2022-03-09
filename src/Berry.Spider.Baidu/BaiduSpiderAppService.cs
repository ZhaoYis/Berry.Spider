using Berry.Spider.Baidu.Impl;
using Berry.Spider.Domain.Shared;
using Volo.Abp.Application.Services;

namespace Berry.Spider.Baidu;

/// <summary>
/// 百度爬虫
/// </summary>
public class BaiduSpiderAppService : ApplicationService, IBaiduSpiderAppService
{
    private IEnumerable<IBaiduSpiderProvider> BaiduSpiderProviders { get; }

    public BaiduSpiderAppService(IEnumerable<IBaiduSpiderProvider> providers)
    {
        this.BaiduSpiderProviders = providers;
    }

    /// <summary>
    /// 执行爬虫
    /// </summary>
    public async Task ExecuteAsync<T>(T request) where T : ISpiderRequest
    {
        foreach (IBaiduSpiderProvider provider in this.BaiduSpiderProviders)
        {
            if (provider is BaiduSpider4RelatedSearchProvider &&
                request.SourceFrom == SpiderSourceFrom.Baidu_RelatedSearch)
            {
                await provider.ExecuteAsync(request);
            }
        }
    }
}