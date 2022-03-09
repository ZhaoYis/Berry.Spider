using Volo.Abp.Application.Services;

namespace Berry.Spider.Application.Spider;

/// <summary>
/// 爬虫服务
/// </summary>
public class SpiderAppService : ApplicationService, ISpiderAppService
{
    /// <summary>
    /// 获取爬虫列表
    /// </summary>
    /// <returns></returns>
    public async Task<List<SpiderDto>> GetListAsync(GetListInput input)
    {
        //TODO：待实现

        return await Task.FromResult(new List<SpiderDto>());
    }
}