namespace Berry.Spider.ToolkitStore.Models;

public class NotificationTaskMessage(bool isRunning)
{
    public bool IsRunning { get; } = isRunning;
}