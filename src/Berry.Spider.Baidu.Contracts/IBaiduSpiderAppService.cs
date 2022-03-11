using Volo.Abp.Application.Services;

namespace Berry.Spider.Baidu;

public interface IBaiduSpiderAppService : IApplicationService
{
    Task ExecuteAsync<T>(T request) where T : ISpiderRequest;

    Task HandleEventAsync<T>(T eventData) where T : ISpiderPullEto;
}