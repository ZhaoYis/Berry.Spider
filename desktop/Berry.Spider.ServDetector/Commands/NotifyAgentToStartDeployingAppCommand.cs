using System.Diagnostics;
using System.Text.Json;
using Berry.Spider.Biz;
using Berry.Spider.Core;
using Berry.Spider.Core.Commands;
using Volo.Abp.DependencyInjection;

namespace Berry.Spider.ServDetector.Commands;

/// <summary>
/// 通知Agent节点开始部署应用
/// </summary>
[CommandName(nameof(RealTimeMessageCode.NOTIFY_AGENT_TO_START_DEPLOYING_APP))]
public class NotifyAgentToStartDeployingAppCommand : IFixedCommand, ITransientDependency
{
    private const string AgentConfigName = "agent.ini";
    private const string ProcessName = "Berry.Spider.Consumers";

    private QiniuDownloadManager QiniuDownloadManager { get; }

    public NotifyAgentToStartDeployingAppCommand(QiniuDownloadManager qiniuDownloadManager)
    {
        this.QiniuDownloadManager = qiniuDownloadManager;
    }

    public async Task ExecuteAsync(CommandLineArgs commandLineArgs)
    {
        NotifyAgentToStartDeployingAppDto? dto = JsonSerializer.Deserialize<NotifyAgentToStartDeployingAppDto>(commandLineArgs.Body);
        if (dto is not null and { RunAppCount: > 0 })
        {
            string deployPath = Path.Combine(AppContext.BaseDirectory, "Download");
            if (!Directory.Exists(deployPath)) Directory.CreateDirectory(deployPath);

            //检查当前机器是否存在部署包
            EasyConfigHelper easyConfigHelper = new EasyConfigHelper(AgentConfigName);
            if (easyConfigHelper.Exists("Install", "Path"))
            {
                //检查当前机器是否存在运行中的即将部署的应用进程，存在则全部停止
                Process[] processes = Process.GetProcessesByName(ProcessName);
                foreach (Process process in processes)
                {
                    process.Kill();
                    await process.WaitForExitAsync();
                }

                //检查版本
                string currentRunVersion = easyConfigHelper.Get("Install", "Version");
                if (currentRunVersion == dto.RunAppInfo.TagName)
                {
                    deployPath = easyConfigHelper.Get("Install", "Path");
                }
                else
                {
                    deployPath = await this.DownloadPackageAsync(deployPath, dto.RunAppInfo);
                }
            }
            else
            {
                deployPath = await this.DownloadPackageAsync(deployPath, dto.RunAppInfo);
            }

            //运行节点
            string deployAppPath = Path.Combine(deployPath, ProcessName, $"{ProcessName}.exe");
            for (int i = 0; i < dto.RunAppCount; i++)
            {
                UserProcessHelper.StartProcessAndBypassUAC(deployAppPath, string.Empty, out UserProcessHelper.PROCESS_INFORMATION pInfo);
                await Task.Delay(100).ConfigureAwait(false);
            }

            //保存配置信息
            easyConfigHelper.Set("Install", new Dictionary<string, string>
            {
                { "Path", deployPath },
                { "Version", dto.RunAppInfo.TagName },
                { "Count", dto.RunAppCount.ToString() }
            });
        }
    }

    /// <summary>
    /// 下载部署包
    /// </summary>
    /// <returns></returns>
    private async Task<string> DownloadPackageAsync(string downloadPath, SpiderAppInfoDto appInfo)
    {
        //下载部署包
        string downloadFilePath = Path.Combine(downloadPath, appInfo.Name);
        if (!Directory.Exists(downloadFilePath)) Directory.CreateDirectory(downloadFilePath);

        string downloadSourceUrl = appInfo.OssKey;
        string fileName = Path.GetFileName(downloadSourceUrl);
        string deployFileName = Path.Combine(downloadFilePath, fileName);
        await this.QiniuDownloadManager.DownloadPrivateFileAsync(downloadSourceUrl, deployFileName);

        //解压
        return ZipHelper.UnZip(deployFileName, downloadFilePath);
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