using Berry.Spider.Core;
using Microsoft.AspNetCore.Mvc;

namespace Berry.Spider;

/// <summary>
/// 爬虫服务
/// </summary>
[Route("api/services/spider")]
public class SpiderController : SpiderControllerBase
{
    private ISpiderAppService SpiderAppService { get; }

    public SpiderController(ISpiderAppService service)
    {
        this.SpiderAppService = service;
    }

    [HttpGet, Route("get")]
    public Task<SpiderDto> GetAsync(int id)
    {
        return this.SpiderAppService.GetAsync(id);
    }

    [HttpGet, Route("get-list")]
    public Task<CustomPagedResultDto<SpiderDto>> GetListAsync(CustomGetListInput input)
    {
        return this.SpiderAppService.GetListAsync(input);
    }

    [HttpGet, Route("get-all")]
    public Task<CustomPagedResultDto<SpiderDto>> GetAllAsync()
    {
        return this.SpiderAppService.GetAllAsync();
    }

    [HttpPost, Route("create")]
    public Task<SpiderDto> CreateAsync(SpiderCreateInput input)
    {
        return this.SpiderAppService.CreateAsync(input);
    }

    [HttpPut, Route("update")]
    public Task<SpiderDto> UpdateAsync(int id, SpiderUpdateInput input)
    {
        return this.SpiderAppService.UpdateAsync(id, input);
    }

    [HttpDelete, Route("delete")]
    public Task DeleteAsync(int id)
    {
        return this.SpiderAppService.DeleteAsync(id);
    }
}