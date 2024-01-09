namespace Berry.Spider.Core.Commands;

public interface ICommandSelector
{
    Type Select(CommandLineArgs commandLineArgs);
}