using ServerCore;
using Shared;
using System.Net;

namespace CS_Server;

// 1. Recv (N)
// 2. ZoneManager (1) -> GameLogic 으로 수정 TODO
// 3. Send (1)
// 4. DB (1)

class Program
{
    static Listener _listener = new Listener();

    static void GameLogicTask()
    {
        while(true)
        {
            GameLogic.Instance.Update();
            Thread.Sleep(0);
        }
    }

    static void DbTask()
    {
        while (true)
        {
            DbTransaction.Instance.Flush();
            Thread.Sleep(0);
        }
    }

    static void NetworkTask()
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

        static void Main(string[] args)
    {
        var configPath = "../../../../config.json";
        ConfigManager.Instance.LoadConfig(configPath);
        DataManager.LoadData();

        GameLogic.Instance.Push(() => {
            GameLogic.Instance.Add(1);
        } );

        

        // DNS (Domain Name System)
        IPAddress ipAddr = DnsUtil.GetLocalIpAddress();
        IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

        _listener.Initialize(endPoint, () => { return SessionManager.Instance.Generate(); });
        Log.Info("Listening...");

        // GameLogicTask
        var gameLogicTask = new Task(GameLogicTask, TaskCreationOptions.LongRunning);
        gameLogicTask.Start();

        // NetworkTask
        var networkTask = new Task(NetworkTask, TaskCreationOptions.LongRunning);
        networkTask.Start();

        DbTask();
    }
}