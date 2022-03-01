
namespace Berry.Spider.Core;

/// <summary>
/// 爬虫基类
/// </summary>
public abstract class SpiderBase
{
    protected SpiderBase(string homePage)
    {
        this.HomePageUrl = homePage;
        
        this.Init();
    }

    /// <summary>
    /// 入口页面
    /// </summary>
    protected string HomePageUrl { get; }

    /// <summary>
    /// 初始化爬虫相关操作
    /// </summary>
    protected virtual void Init()
    {
        
    }

    /// <summary>
    /// 执行爬虫
    /// </summary>
    public abstract Task ExecuteAsync();
}