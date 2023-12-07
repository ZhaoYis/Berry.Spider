using OpenQA.Selenium;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.Core;

/// <summary>
/// 人机验证断定拦截器
/// </summary>
public interface IHumanMachineVerificationInterceptorProvider : ISingletonDependency
{
    Task<bool> LockedAsync(string sourcePage, string lockedPage);

    Task<bool> IsLockedAsync(string sourcePage);
}