using Berry.Spider.Contracts;
using Volo.Abp.Application.Services;

namespace Berry.Spider.TouTiao;

public interface ITouTiaoSpiderAppService : IApplicationService
{
    Task ExecuteAsync<T>(T request) where T : ISpiderRequest;
}