namespace Berry.Spider.Core.Commands;

public class CommandLineArgs
{
    /// <summary>
    /// 命令代码
    /// </summary>
    public string Command { get; set; }

    /// <summary>
    /// 携带的报文
    /// </summary>
    public string Body { get; set; }
}