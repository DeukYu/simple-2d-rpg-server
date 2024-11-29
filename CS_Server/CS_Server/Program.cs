using ServerCore;
using Shared;
using System.Net;

namespace CS_Server;

class Program
{
    static Listener _listener = new Listener();
    static List<System.Timers.Timer> _timers = new List<System.Timers.Timer>();

    static void TickZone(Zone zone, int tick = 1000)
    {
        var timer = new System.Timers.Timer();
        timer.Interval = tick;
        timer.Elapsed += (s, e) => zone.Update();
        timer.AutoReset = true;
        timer.Enabled = true;

        _timers.Add(timer);
    }

    static void Main(string[] args)
    {
        var configPath = "../../../../config.json";
        ConfigManager.Instance.LoadConfig(configPath);
        DataManager.LoadData();

        var zone = ZoneManager.Instance.Add(1);
        TickZone(zone, 50);

        // DNS (Domain Name System)
        IPAddress ipAddr = DnsUtil.GetLocalIpAddress();
        IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

        _listener.Initialize(endPoint, () => { return SessionManager.Instance.Generate(); });
        Log.Info("Listening...");

        while (true)
        {
            DbTransaction.Instance.Flush();
        }
    }
}