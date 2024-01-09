using System.Text.Json;
using Berry.Spider.Biz;
using Berry.Spider.Core.Commands;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.ServDetector.Commands;

/// <summary>
/// 通知Agent节点开始部署应用
/// </summary>
public class NotifyAgentToStartDeployingAppCommand : IFixedCommand, ITransientDependency
{
    public async Task ExecuteAsync(CommandLineArgs commandLineArgs)
    {
        NotifyAgentToStartDeployingAppDto? dto = JsonSerializer.Deserialize<NotifyAgentToStartDeployingAppDto>(commandLineArgs.Body);
        if (dto is not null and { RunAppCount: > 0 })
        {
            //检查当前机器是否存在部署包
            //检查当前机器是否存在运行中的即将部署的应用进程
        }
    }
}

internal class NotifyAgentToStartDeployingAppDto
{
    /// <summary>
    /// 需要启动的应用基础信息
    /// </summary>
    public SpiderAppInfoDto RunAppInfo { get; set; }

    /// <summary>
    /// 启动应用数量
    /// </summary>
    public int RunAppCount { get; set; }
}