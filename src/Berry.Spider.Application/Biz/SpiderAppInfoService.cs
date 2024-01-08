using System.Linq.Dynamic.Core;
using Berry.Spider.Biz;
using Berry.Spider.Domain;
using Volo.Abp.Application.Services;

namespace Berry.Spider.Application;

public class SpiderAppInfoService : ApplicationService, ISpiderAppInfoService
{
    private readonly ISpiderAppInfoRepository _spiderAppInfoRepository;

    public SpiderAppInfoService(ISpiderAppInfoRepository spiderAppInfoRepository)
    {
        _spiderAppInfoRepository = spiderAppInfoRepository;
    }

    /// <summary>
    /// 获取最近几个版本应用信息
    /// </summary>
    /// <param name="top">最近N个</param>
    /// <returns></returns>
    public async Task<List<SpiderAppInfoDto>> GetSpiderAppListAsync(int top = 3)
    {
        var queryable = await _spiderAppInfoRepository.GetQueryableAsync();
        queryable = queryable
            .Where(c => !string.IsNullOrEmpty(c.OssKey))
            .OrderByDescending(c => c.CreatedAt)
            .Take(top);

        var list = await _spiderAppInfoRepository.AsyncExecuter.ToListAsync(queryable);
        return this.ObjectMapper.Map<List<SpiderAppInfo>, List<SpiderAppInfoDto>>(list);
    }

    /// <summary>
    /// 获取某个应用的详细信息
    /// </summary>
    /// <param name="bizNo">业务编码</param>
    /// <returns></returns>
    public async Task<SpiderAppInfoDto> GetSpiderAppInfoAsync(string bizNo)
    {
        SpiderAppInfo appInfo = await _spiderAppInfoRepository.GetAsync(c => c.BizNo == bizNo);
        return this.ObjectMapper.Map<SpiderAppInfo, SpiderAppInfoDto>(appInfo);
    }
}