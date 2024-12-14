using ServerCore;
using Shared;
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
        IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

        _listener.Initialize(endPoint, () => SessionManager.Instance.Generate());
        Log.Info("Listening...");

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