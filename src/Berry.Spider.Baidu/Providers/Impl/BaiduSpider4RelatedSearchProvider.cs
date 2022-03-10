namespace Berry.Spider.Baidu;

/// <summary>
/// 百度：相关推荐
/// </summary>
public class BaiduSpider4RelatedSearchProvider : IBaiduSpiderProvider
{
    public async Task ExecuteAsync<T>(T request) where T : ISpiderRequest
    {
        //TODO:待实现
        
        //采集百度相关搜索并拿出标题
        
        throw new NotImplementedException();
    }
}