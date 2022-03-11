namespace Berry.Spider.Baidu;

public interface IBaiduSpiderProvider
{
    Task ExecuteAsync<T>(T request) where T : ISpiderRequest;

    Task HandleEventAsync<T>(T eventData) where T : ISpiderPullEto;
}