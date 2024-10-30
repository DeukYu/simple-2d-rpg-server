using ServerCore;
using System.Net;
using System.Text;

namespace CS_Server;


class Program
{
    static Listener _listener = new Listener();
    static void Main(string[] args)
    {
        // DNS (Domain Name System)
        IPAddress ipAddr = DnsUtil.GetLocalIpAddress();
        IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

        _listener.Initialize(endPoint, () => { return new ClientSession(); });

        while (true)
        {
            ;
        }

    }
}