using Berry.Spider.Contracts;

namespace Berry.Spider.TouTiao;

/// <summary>
/// 今日头条爬虫
/// </summary>
public class TouTiaoSpiderService : SpiderBaseService, ITouTiaoSpiderService
{
    private IEnumerable<ITouTiaoSpiderProvider> TiaoSpiderProviders { get; }

    public TouTiaoSpiderService(IEnumerable<ITouTiaoSpiderProvider> spiderProviders)
    {
        this.TiaoSpiderProviders = spiderProviders;
    }

    /// <summary>
    /// 执行爬虫
    /// </summary>
    public override async Task ExecuteAsync()
    {
        //TODO 可根据实际情况选择那种具体处理的Provider

        foreach (ITouTiaoSpiderProvider provider in this.TiaoSpiderProviders)
        {
            if (provider is TouTiaoSpider4QuestionProvider)
            {
                await provider.ExecuteAsync();
            }
        }
    }
}