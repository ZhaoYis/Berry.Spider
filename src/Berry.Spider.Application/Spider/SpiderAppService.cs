using Berry.Spider.Domain;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Berry.Spider.Application.Spider;

/// <summary>
/// 爬虫服务
/// </summary>
public class SpiderAppService : CrudAppService<
    SpiderBasic,
    SpiderDto,
    int,
    GetListInput,
    SpiderCreateInput,
    SpiderUpdateInput>, ISpiderAppService
{
    public SpiderAppService(IRepository<SpiderBasic, int> repository) : base(repository)
    {
    }

    public override Task<SpiderDto> CreateAsync(SpiderCreateInput input)
    {
        return base.CreateAsync(input);
    }

    protected override Task<IQueryable<SpiderBasic>> CreateFilteredQueryAsync(GetListInput input)
    {
        return base.CreateFilteredQueryAsync(input);
    }
}