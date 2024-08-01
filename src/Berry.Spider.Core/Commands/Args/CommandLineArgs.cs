namespace Berry.Spider.Core.Commands;

public class CommandLineArgs(string command, object body)
{
    /// <summary>
    /// 命令代码
    /// </summary>
    public string Command { get; init; } = command;

    /// <summary>
    /// 携带的报文
    /// </summary>
    public object Body { get; init; } = body;
}