using Berry.Spider.Contracts;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

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

    protected override void Init()
    {
        new DriverManager().SetUpDriver(new ChromeConfig());
        base.Init();
    }

    /// <summary>
    /// 执行爬虫
    /// </summary>
    public override async Task ExecuteAsync()
    {
        //TODO 可根据实际情况选择那种具体处理的Provider

        foreach (ITouTiaoSpiderProvider provider in this.TiaoSpiderProviders)
        {
            await provider.ExecuteAsync();
        }
    }
}