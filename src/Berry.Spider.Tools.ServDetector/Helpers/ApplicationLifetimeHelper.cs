using System.Text;
using Berry.Spider.Common;

namespace Berry.Spider.Tools.ServDetector;

public static class ApplicationLifetimeHelper
{
    public static string? Build(List<ApplicationLifetimeData> applicationLifetimeList)
    {
        //异常节点数
        int notOkNodeCount = applicationLifetimeList.Count(a => !a.AreYouOk || a.IsOverTime);
        if (notOkNodeCount == 0)
        {
            return default;
        }
        else
        {
            StringBuilder builder = new StringBuilder("## 采集节点异常告警消息\n");
            builder.AppendLine($"采集共有<font color='warning'>{notOkNodeCount}</font>个节点出现异常，请及时关注");

            //根据机器分组
            var groupByMachineName = applicationLifetimeList
                .Where(a => !a.AreYouOk || a.IsOverTime)
                .GroupBy(a => a.MachineName);

            foreach (IGrouping<string, ApplicationLifetimeData> item in groupByMachineName)
            {
                string machineName = item.Key;
                List<ApplicationLifetimeData> list = item.OrderByDescending(c => c.Time).ToList();

                builder.AppendLine($"### {machineName}");
                foreach (ApplicationLifetimeData data in list)
                {
                    builder.AppendLine($">进程ID：{data.ProcessId}");
                    builder.AppendLine($">进程名称：{data.ProcessName}");
                    builder.AppendLine($">内存占用：{data.MemoryUsage}");
                    builder.AppendLine($">CPU占用：{data.CpuUsage}");
                    builder.AppendLine($">探测时间：{data.Time:yyyy-MM-dd HH:mm:ss}\n");
                }
            }

            return builder.ToString();
        }
    }
}