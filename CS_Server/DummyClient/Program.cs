
using ServerCore;
using System.Net;
using System.Text;

namespace DummyClient;

class Program
{
    static void Main(string[] args)
    {
        // DNS (Domain Name System)
        IPAddress ipAddr = DnsUtil.GetLocalIpAddress();
        IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

        Connector connector = new Connector();

        connector.Connect(endPoint, () => { return new ServerSession(); });

        while (true)
        {
            try
            {
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            Thread.Sleep(100);
        }
    }
}