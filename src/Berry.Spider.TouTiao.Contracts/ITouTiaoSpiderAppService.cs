using Volo.Abp.Application.Services;

namespace Berry.Spider.TouTiao;

public interface ITouTiaoSpiderAppService : IApplicationService
{
    Task PublishAsync<T>(T request) where T : ISpiderRequest;

    Task HandleEventAsync<T>(T eventData) where T : ISpiderPullEto;
}