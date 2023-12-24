using System.Text;
using Berry.Spider.Common;

namespace Berry.Spider.ServDetector.Helpers;

public static class ApplicationLifetimeHelper
{
    public static string? Build(List<ApplicationLifetimeData> applicationLifetimeList)
    {
        //异常节点数
        int notOkNodeCount = applicationLifetimeList.Count(a => !a.AreYouOk || a.IsOverTime());
        if (notOkNodeCount == 0)
        {
            return default;
        }
        else
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("# 采集节点异常告警消息");
            builder.AppendLine($"采集共有<font color='warning'>{notOkNodeCount}</font>个节点出现异常，请及时关注¥_¥");
            builder.AppendLine($"推送时间：<font color='info'>{DateTime.Now:yyyy-MM-dd HH:mm:ss}</font>{Environment.NewLine}");

            //根据机器分组
            var groupByMachineName = applicationLifetimeList
                .Where(a => !a.AreYouOk || a.IsOverTime())
                .GroupBy(a => a.MachineName);

            foreach (IGrouping<string, ApplicationLifetimeData> item in groupByMachineName)
            {
                string machineName = item.Key;
                List<ApplicationLifetimeData> list = item.OrderByDescending(c => c.Time).ToList();

                builder.AppendLine($"**机器名称：<font color='warning'>{machineName}</font>**");
                foreach (ApplicationLifetimeData data in list)
                {
                    builder.AppendLine($">进程ID：{data.ProcessId}");
                    builder.AppendLine($">内存占用：{data.MemoryUsage}");
                    builder.AppendLine($">CPU占用：{data.CpuUsage}");
                    builder.AppendLine($">探测时间：{data.Time:yyyy-MM-dd HH:mm:ss}");
                    builder.Append($"{Environment.NewLine}");
                }

                builder.Append($"{Environment.NewLine}");
            }

            string s = builder.ToString();
            return s;
        }
    }
}