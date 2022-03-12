using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;

namespace Berry.Spider;

/// <summary>
/// 爬虫服务
/// </summary>
[Route("api/services/spider")]
public class SpiderController : SpiderControllerBase, ISpiderAppService
{
    private ISpiderAppService SpiderAppService { get; }

    public SpiderController(ISpiderAppService service)
    {
        this.SpiderAppService = service;
    }

    [HttpGet, Route("get")]
    public async Task<SpiderDto> GetAsync(int id)
    {
        return await this.SpiderAppService.GetAsync(id);
    }

    [HttpGet, Route("get-list")]
    public async Task<PagedResultDto<SpiderDto>> GetListAsync(GetListInput input)
    {
        return await this.SpiderAppService.GetListAsync(input);
    }

    [HttpPost, Route("create")]
    public async Task<SpiderDto> CreateAsync(SpiderCreateInput input)
    {
        return await this.SpiderAppService.CreateAsync(input);
    }

    [HttpPut, Route("update")]
    public async Task<SpiderDto> UpdateAsync(int id, SpiderUpdateInput input)
    {
        return await this.SpiderAppService.UpdateAsync(id, input);
    }

    [HttpDelete, Route("delete")]
    public async Task DeleteAsync(int id)
    {
        await this.SpiderAppService.DeleteAsync(id);
    }
}