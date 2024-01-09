using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.Core.Commands;

public class CommandSelector : ICommandSelector, ITransientDependency
{
    private CommandOptions CommandOptions { get; }

    public CommandSelector(IOptions<CommandOptions> options)
    {
        this.CommandOptions = options.Value;
    }

    public Type Select(CommandLineArgs commandLineArgs)
    {
        if (commandLineArgs.Command.IsNullOrWhiteSpace())
        {
            return typeof(NullCommand);
        }

        return this.CommandOptions.Commands.GetOrDefault(commandLineArgs.Command) ?? typeof(NullCommand);
    }
}