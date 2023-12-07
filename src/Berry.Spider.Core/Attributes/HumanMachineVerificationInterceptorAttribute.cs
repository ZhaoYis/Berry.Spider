using AspectCore.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;

namespace Berry.Spider.Core;

/// <summary>
/// 人机验证（AOP的方式）
/// </summary>
public class HumanMachineVerificationInterceptorAttribute : AbstractInterceptorAttribute
{
    public override async Task Invoke(AspectContext aspectContext, AspectDelegate next)
    {
        var provider = aspectContext.ServiceProvider.GetService<IHumanMachineVerificationInterceptorProvider>();
        if (provider is not null)
        {
            object targetUrl = aspectContext.Parameters.First();
            if (targetUrl is string sourcePage)
            {
                //检查是否处于人机验证资源锁定阶段
                if (await provider.IsLockedAsync(sourcePage)) return;
            }
        }

        await next(aspectContext);
    }
}