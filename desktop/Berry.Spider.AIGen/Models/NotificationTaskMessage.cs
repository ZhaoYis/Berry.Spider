namespace Berry.Spider.AIGen.Models;

public class NotificationTaskMessage(bool isRunning)
{
    public bool IsRunning { get; } = isRunning;
}