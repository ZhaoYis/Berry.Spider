using Volo.Abp.Application.Services;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.Contracts;

public interface ISpiderAppService : IApplicationService, ITransientDependency
{
    Task<List<SpiderDto>> GetListAsync(GetListInput input);
}