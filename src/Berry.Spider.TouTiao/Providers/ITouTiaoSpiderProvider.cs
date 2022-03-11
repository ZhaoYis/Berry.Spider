namespace Berry.Spider.TouTiao;

public interface ITouTiaoSpiderProvider
{
    Task PublishAsync<T>(T request) where T : ISpiderRequest;

    Task HandleEventAsync<T>(T eventData) where T : ISpiderPullEto;
}