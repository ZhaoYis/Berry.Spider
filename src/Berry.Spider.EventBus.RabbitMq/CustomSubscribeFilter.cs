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
    public override Task OnSubscribeExecutingAsync(ExecutingContext context)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// 订阅方法执行后
    /// </summary>
    public override Task OnSubscribeExecutedAsync(ExecutedContext context)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// 订阅方法执行异常
    /// </summary>
    public override Task OnSubscribeExceptionAsync(ExceptionContext context)
    {
        //忽略异常
        context.ExceptionHandled = true;
        return Task.CompletedTask;
    }
}