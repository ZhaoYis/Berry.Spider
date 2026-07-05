namespace Berry.Spider.DesktopProjectDemo.Models;

public class NotificationTaskMessage(bool isRunning)
{
    public bool IsRunning { get; } = isRunning;
}