namespace Berry.Spider.Core.Commands;

[AttributeUsage(AttributeTargets.Class)]
public class CommandNameAttribute : Attribute
{
    public string Command { get; set; }

    public CommandNameAttribute(string command)
    {
        this.Command = command;
    }
}