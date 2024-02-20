// using Berry.Spider.Core;
// using Volo.Abp.Application.Services;
// using Volo.Abp.DependencyInjection;
//
// namespace Berry.Spider;
//
// public interface ISpiderAppService : ICrudAppService<
//     SpiderDto,
//     int,
//     GetListInput,
//     SpiderCreateInput,
//     SpiderUpdateInput>, ITransientDependency
// {
//     /// <summary>
//     /// 获取分页记录
//     /// </summary>
//     /// <returns></returns>
//     new Task<CustomPagedResultDto<SpiderDto>> GetListAsync(CustomGetListInput input);
//
//     /// <summary>
//     /// 获取所有记录
//     /// </summary>
//     /// <returns></returns>
//     Task<CustomPagedResultDto<SpiderDto>> GetAllAsync();
// }