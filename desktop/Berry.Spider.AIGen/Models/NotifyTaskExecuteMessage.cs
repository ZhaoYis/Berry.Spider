namespace Berry.Spider.AIGen.Models;

public class NotifyTaskExecuteMessage(bool isRunning)
{
    public bool IsRunning { get; } = isRunning;
}