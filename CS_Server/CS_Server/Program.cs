using ServerCore;
using Shared;
using Shared.DB;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace CS_Server;

// 1. Recv (N)
// 2. GameLogic (1)
// 3. Send (1)
// 4. DB (1)

class Program
{
    static Listener _listener = new Listener();

    static void GameLogicTask()
    {
        try
        {
            while (true)
            {
                GameLogic.Instance.ProcessJobs();
                Thread.Sleep(0);
            }
        }
        catch (Exception e)
        {
            Log.Error(e.ToString());
        }
    }
    static void NetworkTask()
    {
        try
        {
            while (true)
            {
                var sessions = SessionManager.Instance.GetSessions();
                foreach (ClientSession session in sessions)
                {
                    session.FlushSend();
                }
                Thread.Sleep(0);
            }
        }
        catch (Exception e)
        {
            Log.Error(e.ToString());
        }
    }

    static void DbTask()
    {
        try
        {
            while (true)
            {
                DbTransaction.Instance.ProcessJobs();
                Thread.Sleep(0);
            }
        }
        catch (Exception e)
        {
            Log.Error(e.ToString());
        }
    }

    private static void StartServerInfoTask()
    {
        var t = new System.Timers.Timer();
        t.AutoReset = true;
        t.Elapsed += new System.Timers.ElapsedEventHandler((s, e) =>
        {
            using (var sharedDB = new SharedDB())
            {
                var serverConfigInfo = sharedDB.ServerConfigInfo.FirstOrDefault();
                if (serverConfigInfo != null)
                {
                    serverConfigInfo.IpAddress = DnsUtil.GetLocalIpAddress().ToString();
                    serverConfigInfo.Port = ConfigManager.Instance.ServerConfig.ServerPort;
                    sharedDB.SaveChangesEx();

                }
                else
                {
                    sharedDB.ServerConfigInfo.Add(new ServerConfigInfo
                    {
                        Name = Program.Name,
                        IpAddress = DnsUtil.GetLocalIpAddress().ToString(),
                        Port = ConfigManager.Instance.ServerConfig.ServerPort,
                        Congestion = SessionManager.Instance.GetCongestion(),
                    });
                    sharedDB.SaveChangesEx();
                }
            }
        });
        t.Interval = 10 * 1000;
        t.Start();
    }

    public static string Name { get; } = "로컬 서버";
    public static int Port { get; } = 7777;

    static void Main(string[] args)
    {
        var configPath = "../../../../config.json";
        ConfigManager.Instance.LoadConfig(configPath);
        DataManager.LoadData();

        // TODO : Zone 생성하는데, 여러 Zone을 만들어서 테스트해보기
        GameLogic.Instance.ScheduleJob(() =>
        {
            GameLogic.Instance.Add(1);
        });

        // DNS (Domain Name System)
        IPAddress ipAddr = DnsUtil.GetLocalIpAddress();
        var port = ConfigManager.Instance.ServerConfig.ServerPort;
        IPEndPoint endPoint = new IPEndPoint(ipAddr, port);

        _listener.Initialize(endPoint, () => SessionManager.Instance.Generate());
        Log.Info("Listening...");

        StartServerInfoTask();

        // DbTask : Main Trhead
        var dbTask = new Task(DbTask, TaskCreationOptions.LongRunning);
        dbTask.Start();

        // NetworkTask
        var networkTask = new Task(NetworkTask, TaskCreationOptions.LongRunning);
        networkTask.Start();

        // GameLogicTask
        GameLogicTask();
    }
}