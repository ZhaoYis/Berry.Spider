using DotNetCore.CAP.Filter;

namespace Berry.Spider.EventBus.RabbitMq;

/// <summary>
/// 自定义过滤器
/// </summary>
public class CustomSubscribeFilter : SubscribeFilter
{
    /// <summary>
    /// 订阅方法执行前
    /// </summary>
    public override void OnSubscribeExecuting(ExecutingContext context)
    {
    }

    /// <summary>
    /// 订阅方法执行后
    /// </summary>
    public override void OnSubscribeExecuted(ExecutedContext context)
    {
    }

    /// <summary>
    /// 订阅方法执行异常
    /// </summary>
    public override void OnSubscribeException(ExceptionContext context)
    {
        //忽略异常
        context.ExceptionHandled = true;
    }
}