namespace Berry.Spider.Weixin.Work;

public class WeixinWorkRobotOptions
{
    public string BaseAddress { get; set; }

    public string AppKey { get; set; }

    public int TimerPeriod { get; set; } = 10 * 1000;
}