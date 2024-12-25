namespace Berry.Spider.AIGenPlus.Models;

public class NotificationTaskMessage(bool isRunning)
{
    public bool IsRunning { get; } = isRunning;
}