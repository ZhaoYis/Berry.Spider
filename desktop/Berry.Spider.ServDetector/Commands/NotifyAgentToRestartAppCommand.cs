using System.Diagnostics;
using Berry.Spider.Core;
using Berry.Spider.Core.Commands;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.ServDetector.Commands;

/// <summary>
/// 通知Agent节点重启所有应用
/// </summary>
[CommandName(nameof(RealTimeMessageCode.NOTIFY_AGENT_TO_RESTART_APP))]
public class NotifyAgentToRestartAppCommand : IFixedCommand, ITransientDependency
{
    private const string AgentConfigName = "agent.ini";
    private const string ProcessName = "Berry.Spider.Consumers";

    public async Task ExecuteAsync(CommandLineArgs commandLineArgs)
    {
        //检查当前机器是否存在运行中的即将部署的应用进程，存在则全部停止
        EasyConfigHelper easyConfigHelper = new EasyConfigHelper(AgentConfigName);
        if (easyConfigHelper.Exists("Install", "Path"))
        {
            string deployPath = easyConfigHelper.Get("Install", "Path");
            int runAppCount = int.Parse(easyConfigHelper.Get("Install", "Count"));

            //检查当前机器是否存在运行中的即将部署的应用进程，存在则全部停止
            Process[] processes = Process.GetProcessesByName(ProcessName);
            foreach (Process process in processes)
            {
                process.Kill();
            }

            //运行节点
            string deployAppPath = Path.Combine(deployPath, ProcessName, $"{ProcessName}.exe");
            for (int i = 0; i < runAppCount; i++)
            {
                Process.Start(deployAppPath);
                await Task.Delay(100).ConfigureAwait(false);
            }
        }
    }
}