namespace Berry.Spider.EventBus.RabbitMq;

public class EventBusRabbitMqOptions
{
    public string HostName { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Port { get; set; }
    public string VirtualHost { get; set; }

    /// <summary>
    /// 是否启用发布确认
    /// </summary>
    public bool PublishConfirms { get; set; } = false;
}