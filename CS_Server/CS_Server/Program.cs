using ServerCore;
using System.Net;
using System.Text;

namespace CS_Server;


class Program
{
    static Listener _listener = new Listener();
    public static Zone Room = new Zone();
    static void FlushRoom()
    {
        Room.Push(() => Room.Flush());
        JobTimer.Instance.Push(FlushRoom, 250);
    }
    static void Main(string[] args)
    {
        // DNS (Domain Name System)
        IPAddress ipAddr = DnsUtil.GetLocalIpAddress();
        IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

        _listener.Initialize(endPoint, () => { return SessionManager.Instance.Generate(); });

        JobTimer.Instance.Push(FlushRoom);

        while (true)
        {
            JobTimer.Instance.Flush();
        }
    }
}