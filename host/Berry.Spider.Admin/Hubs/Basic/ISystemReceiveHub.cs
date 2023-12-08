namespace Berry.Spider.Admin;

public interface ISystemReceiveHub
{
    /// <summary>
    /// 终端接收服务器发送的系统级别消息
    /// </summary>
    /// <returns></returns>
    Task ReceiveSystemMessageAsync(SystemReceiveDto receive);
}