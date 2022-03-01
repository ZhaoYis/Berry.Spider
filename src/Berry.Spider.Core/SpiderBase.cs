
namespace Berry.Spider.Core;

/// <summary>
/// 爬虫基类
/// </summary>
public abstract class SpiderBase
{
    protected SpiderBase()
    {
        this.Init();
    }

    /// <summary>
    /// 入口页面
    /// </summary>
    protected abstract string HomePageUrl { get; }

    /// <summary>
    /// 初始化爬虫
    /// </summary>
    private void Init()
    {
        
    }

    /// <summary>
    /// 执行爬虫
    /// </summary>
    public abstract Task ExecuteAsync();
}