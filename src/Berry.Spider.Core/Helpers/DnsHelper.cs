using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Berry.Spider.Core;

public static class DnsHelper
{
    /// <summary>
    /// 返回主机名称
    /// </summary>
    /// <returns></returns>
    public static string GetHostName()
    {
        return Dns.GetHostName();
    }

    /// <summary>
    /// 返回当前机器IP地址。多个使用,分开
    /// </summary>
    /// <returns></returns>
    public static string GetIpV4s()
    {
        try
        {
            var ipAddresses = Dns.GetHostAddresses(GetHostName());
            string[] addrList = ipAddresses.Where(x => x.AddressFamily == AddressFamily.InterNetwork).Select(ipAddress => ipAddress.ToString()).ToArray();
            return string.Join(",", addrList);
        }
        catch
        {
            return "-";
        }
    }

    /// <summary>
    /// 获取MAC地址
    /// </summary>
    /// <returns></returns>
    public static string GetMacAddress()
    {
        List<string> macList = new List<string>();
        NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
        foreach (NetworkInterface networkInterface in networkInterfaces)
        {
            string macAddress = networkInterface.GetPhysicalAddress().ToString();
            macList.Add(macAddress);
        }

        return string.Join(",", macList.Where(mac => !string.IsNullOrEmpty(mac)));
    }
}