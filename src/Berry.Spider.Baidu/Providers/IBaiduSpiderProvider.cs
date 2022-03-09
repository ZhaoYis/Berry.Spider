using Volo.Abp.DependencyInjection;

namespace Berry.Spider.Baidu;

public interface IBaiduSpiderProvider : ITransientDependency
{
    Task ExecuteAsync<T>(T request) where T : ISpiderRequest;
}