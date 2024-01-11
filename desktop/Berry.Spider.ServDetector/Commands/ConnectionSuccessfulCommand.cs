using Berry.Spider.Core;
using Berry.Spider.Core.Commands;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.ServDetector.Commands;

[CommandName(nameof(RealTimeMessageCode.CONNECTION_SUCCESSFUL))]
public class ConnectionSuccessfulCommand : IFixedCommand, ITransientDependency
{
    public async Task ExecuteAsync(CommandLineArgs commandLineArgs)
    {
        await Task.CompletedTask;
    }
}