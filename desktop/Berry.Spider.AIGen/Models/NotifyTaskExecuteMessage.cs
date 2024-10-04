namespace Berry.Spider.AIGen.Models;

public class NotifyTaskExecuteMessage
{
    public NotifyTaskExecuteMessage(bool isRunning)
    {
        this.IsRunning = isRunning;
    }

    public bool IsRunning { get; set; }
}