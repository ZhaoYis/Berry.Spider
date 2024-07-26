namespace Berry.Spider.Core.Commands;

public class CommandLineArgs
{
    public CommandLineArgs(string command, object body)
    {
        Command = command;
        Body = body;
    }

    /// <summary>
    /// 命令代码
    /// </summary>
    public string Command { get; init; }

    /// <summary>
    /// 携带的报文
    /// </summary>
    public object Body { get; init; }
}