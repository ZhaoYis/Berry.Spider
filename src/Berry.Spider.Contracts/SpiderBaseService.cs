namespace Berry.Spider.Contracts;

/// <summary>
/// 爬虫服务基类
/// </summary>
public abstract class SpiderBaseService
{
    protected SpiderBaseService()
    {
        this.Init();
    }

    /// <summary>
    /// 初始化爬虫相关操作
    /// </summary>
    protected virtual void Init()
    {
    }
}