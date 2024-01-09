namespace Berry.Spider.Core.Commands;

public interface IFixedCommand
{
    Task ExecuteAsync(CommandLineArgs commandLineArgs);
}