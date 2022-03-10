namespace Berry.Spider.TouTiao;

public interface ITouTiaoSpiderProvider
{
    Task ExecuteAsync<T>(T request) where T : ISpiderRequest;
}