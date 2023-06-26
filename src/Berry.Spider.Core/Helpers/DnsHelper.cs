using System.Net;
using System.Net.Sockets;

namespace Berry.Spider.Core.Helpers;

public static class DnsHelper
{
    public static string GetHostName()
    {
        return Dns.GetHostName();
    }

    public static string[] GetIpV4s()
    {
        try
        {
            var ipAddresses = Dns.GetHostAddresses(Dns.GetHostName());
            return ipAddresses.Where(x => x.AddressFamily == AddressFamily.InterNetwork).Select(ipAddress => ipAddress.ToString()).ToArray();
        }
        catch
        {
            return Array.Empty<string>();
        }
    }
}