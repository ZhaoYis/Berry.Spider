using DotNetCore.CAP.Filter;

namespace Berry.Spider.EventBus.RabbitMq;

public class CustomSubscribeFilter : SubscribeFilter
{
    public override void OnSubscribeExecuting(ExecutingContext context)
    {
        // 订阅方法执行前
    }

    public override void OnSubscribeExecuted(ExecutedContext context)
    {
        // 订阅方法执行后
    }

    public override void OnSubscribeException(ExceptionContext context)
    {
        // 订阅方法执行异常
    }
}