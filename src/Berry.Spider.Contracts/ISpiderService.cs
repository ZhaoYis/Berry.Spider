using Volo.Abp.DependencyInjection;

namespace Berry.Spider.Contracts;

public interface ISpiderService : ITransientDependency
{
    Task ExecuteAsync<T>(T request) where T : ISpiderRequest;
}