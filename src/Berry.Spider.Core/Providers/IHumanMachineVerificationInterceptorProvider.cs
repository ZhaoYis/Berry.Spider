using OpenQA.Selenium;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.Core;

/// <summary>
/// 人机验证断定拦截器
/// </summary>
public interface IHumanMachineVerificationInterceptorProvider : ITransientDependency
{
    Task InvokeAsync(IWebDriver webDriver);
    
    Task<bool> IsLockedAsync();
}