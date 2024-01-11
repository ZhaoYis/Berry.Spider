using Berry.Spider.Core;
using Berry.Spider.Core.Commands;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.ServDetector.Commands;

[CommandName(nameof(RealTimeMessageCode.SYSTEM_MESSAGE))]
public class SystemMessageCommand : IFixedCommand, ITransientDependency
{
    public async Task ExecuteAsync(CommandLineArgs commandLineArgs)
    {
        await Task.CompletedTask;
    }
}