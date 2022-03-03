using Volo.Abp.DependencyInjection;

namespace Berry.Spider.TouTiao;

public interface ITouTiaoSpiderProvider : ITransientDependency
{
    Task ExecuteAsync();
}