using Volo.Abp.Application.Services;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider;

public interface ISpiderAppService : ICrudAppService<
    SpiderDto,
    int,
    GetListInput,
    SpiderCreateInput,
    SpiderUpdateInput>, ITransientDependency
{
}