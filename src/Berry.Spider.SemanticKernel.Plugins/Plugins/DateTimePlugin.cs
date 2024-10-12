using System.ComponentModel;
using Microsoft.SemanticKernel;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.SemanticKernel.Plugins;

public class DateTimePlugin : ITransientDependency
{
    /// <summary>
    /// 获取当前系统时间
    /// </summary>
    /// <returns>当前系统时间</returns>
    [KernelFunction("get_current_datetime")]
    [Description("获取当前系统时间")]
    [return: Description("当前系统时间")]
    public Task<string> GetCurrentDateTimeAsync()
    {
        return Task.FromResult(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
    }
}