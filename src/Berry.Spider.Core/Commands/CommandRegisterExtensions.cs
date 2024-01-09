using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Berry.Spider.Core.Commands;

public static class CommandRegisterExtensions
{
    public static void RegisterCommand(this IServiceCollection services, Action<CommandOptions>? action)
    {
        CommandOptions commandOptions = new CommandOptions();
        action?.Invoke(commandOptions);

        services.Configure<CommandOptions>(opt =>
        {
            foreach (var command in commandOptions.Commands)
            {
                opt.Commands.Add(command.Key, command.Value);
            }

            opt.Commands.Add(nameof(NullCommand), typeof(NullCommand));
        });

        //注册命令实现
        foreach (var command in commandOptions.Commands)
        {
            services.TryAddTransient(command.Value);
        }

        services.TryAddTransient<ICommandSelector, CommandSelector>();
    }
}