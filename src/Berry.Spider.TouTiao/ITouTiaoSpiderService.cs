using Volo.Abp.DependencyInjection;

namespace Berry.Spider.TouTiao;

public interface ITouTiaoSpiderService : ITransientDependency
{
    Task ExecuteAsync();
}