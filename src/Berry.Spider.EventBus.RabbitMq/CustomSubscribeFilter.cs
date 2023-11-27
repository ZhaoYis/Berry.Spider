using System.Threading.Tasks;
using Berry.Spider.FreeRedis;
using DotNetCore.CAP.Filter;
using DotNetCore.CAP.Messages;
using Volo.Abp;

namespace Berry.Spider.EventBus.RabbitMq;

/// <summary>
/// 自定义过滤器
/// </summary>
public class CustomSubscribeFilter : SubscribeFilter
{
    private readonly IRedisService _redisService;

    public CustomSubscribeFilter(IRedisService redisService)
    {
        _redisService = redisService;
    }

    /// <summary>
    /// 订阅方法执行前
    /// </summary>
    public override Task OnSubscribeExecutingAsync(ExecutingContext context)
    {
        // bool tryGetIsSucc = context.DeliverMessage.Headers.TryGetValue(Headers.MessageId, out string? messageId);
        // if (tryGetIsSucc && !string.IsNullOrEmpty(messageId))
        // {
        //     bool result = await _redisService.SetAsync(Headers.MessageId, messageId);
        //     if (!result)
        //     {
        //         throw new BusinessException($"消息{messageId}已被处理.");
        //     }
        // }
        return base.OnSubscribeExecutingAsync(context);
    }

    /// <summary>
    /// 订阅方法执行后
    /// </summary>
    public override Task OnSubscribeExecutedAsync(ExecutedContext context)
    {
        return base.OnSubscribeExecutedAsync(context);
    }

    /// <summary>
    /// 订阅方法执行异常
    /// </summary>
    public override Task OnSubscribeExceptionAsync(ExceptionContext context)
    {
        //忽略异常
        context.ExceptionHandled = true;
        return base.OnSubscribeExceptionAsync(context);
    }
}