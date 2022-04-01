using Berry.Spider.Core;
using Berry.Spider.Domain;
using Volo.Abp.Application.Dtos;
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

    public new async Task<CustomPagedResultDto<SpiderDto>> GetListAsync(CustomGetListInput input)
    {
        CustomPagedResultDto<SpiderDto> result = new CustomPagedResultDto<SpiderDto>();

        PagedResultDto<SpiderDto> pagedResultDto = await base.GetListAsync(new GetListInput
            { SkipCount = input.Page * input.PageSize, MaxResultCount = input.PageSize });
        result.Total = pagedResultDto.TotalCount;
        result.Items = pagedResultDto.Items;

        return result;
    }

    public async Task<CustomPagedResultDto<SpiderDto>> GetAllAsync()
    {
        CustomPagedResultDto<SpiderDto> result = new CustomPagedResultDto<SpiderDto>();

        List<SpiderBasic> list = await this.Repository.GetListAsync(c => !c.IsDeleted);
        result.Total = list.Count;
        result.Items = this.ObjectMapper.Map<List<SpiderBasic>, List<SpiderDto>>(list);

        return result;
    }
}