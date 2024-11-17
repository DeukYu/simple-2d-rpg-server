using ServerCore;
using System.Net;

namespace CS_Server;

class Program
{
    static Listener _listener = new Listener();
    static void Main(string[] args)
    {
        ConfigManager.LoadConfig();
        DataManager.LoadData();

        ZoneManager.Instance.Add(1);

        // DNS (Domain Name System)
        IPAddress ipAddr = DnsUtil.GetLocalIpAddress();
        IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

        _listener.Initialize(endPoint, () => { return SessionManager.Instance.Generate(); });

        while (true)
        {
            // TODO : 임시로 1초에 한번씩 업데이트를 호출한다.
            var zone = ZoneManager.Instance.FindZone(1);
            if (zone == null)
            {
                Log.Error("Main: Zone is null");
                return;
            }
            zone.Push(zone.Update);
            Thread.Sleep(100);
        }
    }
}