using System.Net;

namespace ServerCore;

public static class DnsUtil
{
    public static IPAddress GetLocalIpAddress()
    {
        string hostName = Dns.GetHostName();
        var ipHost = Dns.GetHostEntry(hostName);
        foreach (var ipAddr in ipHost.AddressList)
        {
            if (ipAddr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                return ipAddr;
            }
        }

        throw new Exception("Local ip address not found.");
    }
}
