using Volo.Abp.DependencyInjection;

namespace Berry.Spider.Core.Commands;

public class NullCommand : IFixedCommand, ITransientDependency
{
    public Task ExecuteAsync(CommandLineArgs commandLineArgs)
    {
        return Task.CompletedTask;
    }
}