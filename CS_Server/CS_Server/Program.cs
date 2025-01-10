using ServerCore;
using ServerCore.WAL;
using Shared;
using Shared.DB;
using System.Net;

namespace CS_Server;

// 1. Recv (N)
// 2. GameLogic (1)
// 3. Send (1)
// 4. DB (1)
// 5. Server Status Update (1)
class Program
{
    private static Listener _listener = new Listener();
    private static CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    static async Task Main(string[] args)
    {
        // 초기 설정 로드
        InitializeServerConfig();

        // GameLogic 관련 초기화
        InitializeGameLogic();

        // 서버 네트워크 설정
        InitializeListener();

        // 서버 상태 업데이트 작업
        StartServerInfoTask(_cancellationTokenSource.Token);

        // Task 실행
        var gameLogicTask = RunGameLogicTask(_cancellationTokenSource.Token);
        var networkTask = RunNetworkTask(_cancellationTokenSource.Token);
        var dbTask = RunDbTask(_cancellationTokenSource.Token);

        // Ctrl + C 종료 이벤트 처리
        Console.CancelKeyPress += (s, e) =>
        {
            e.Cancel = true;
            ShutdownServer();
        };

        // 모든 작업이 종료될 때까지 대기
        await Task.WhenAll(dbTask, networkTask, gameLogicTask);
    }

    #region Initialize
    private static void InitializeServerConfig()
    {
        try
        {
            var configPath = "../../../../config.json";
            ConfigManager.Instance.LoadConfig(configPath);
            DataManager.LoadData();
        }
        catch (Exception e)
        {
            Log.Error($"[InitializeServerConfig] 설정 파일 로드 실패: {e.Message}");
            throw;
        }
    }
    private static void InitializeGameLogic()
    {
        try
        {
            GameLogic.Instance.ScheduleJob(() =>
            {
                GameLogic.Instance.Init();
            });
        }
        catch (Exception e)
        {
            Log.Error($"[InitializeGameLogic] 게임 로직 초기화 실패: {e.Message}");
            throw;
        }
    }
    private static void InitializeListener()
    {
        try
        {
            // DNS (Domain Name System)
            IPAddress ipAddr = DnsUtil.GetLocalIpAddress();
            var port = ConfigManager.Instance.ServerConfig.ServerPort;
            IPEndPoint endPoint = new IPEndPoint(ipAddr, port);

            _listener.Initialize(endPoint, () => SessionManager.Instance.Generate());
            Log.Info("Listening...");
        }
        catch (Exception e)
        {
            Log.Error($"[InitializeListener] Listener 초기화 실패: {e.Message}");
            throw;
        }
    }
    #endregion
    #region Task
    private static async Task RunTaskAsync(Func<Task> taskFunc, string taskName, CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await taskFunc();
                await Task.Delay(1, cancellationToken);
            }
        }
        catch (Exception e)
        {
            Log.Error($"[RunTaskAsync] {taskName} 실패: {e}");
        }
    }
    private static async Task RunGameLogicTask(CancellationToken cancellationToken)
    {
        await RunTaskAsync(async () =>
        {
            GameLogic.Instance.Update();
            await Task.CompletedTask;
        }, "GameLogic", cancellationToken);
    }
    private static async Task RunNetworkTask(CancellationToken cancellationToken)
    {
        await RunTaskAsync(async () =>
        {
            var sessions = SessionManager.Instance.GetSessions();
            foreach (ClientSession session in sessions)
            {
                session.FlushSend();
            }
            await Task.CompletedTask;
        }, "Network", cancellationToken);
    }
    private static async Task RunDbTask(CancellationToken cancellationToken)
    {
        await RunTaskAsync(() =>
        {
            DbTransaction.Instance.ProcessJobs();
            return Task.CompletedTask;
        }, "DB", cancellationToken);
    }
    private static void StartServerInfoTask(CancellationToken cancellationToken)
    {
        Task.Run(async () =>
        {
            while (true)
            {
                try
                {
                    using (var sharedDB = new SharedDB())
                    {
                        var serverConfigInfo = sharedDB.ServerConfigInfo.FirstOrDefault();
                        if (serverConfigInfo != null)
                        {
                            serverConfigInfo.IpAddress = DnsUtil.GetLocalIpAddress().ToString();
                            serverConfigInfo.Port = ConfigManager.Instance.ServerConfig.ServerPort;
                            serverConfigInfo.Congestion = SessionManager.Instance.GetCongestion();
                        }
                        else
                        {
                            sharedDB.ServerConfigInfo.Add(new ServerConfigInfo
                            {
                                Name = ConfigManager.Instance.ServerConfig.ServerName,
                                IpAddress = DnsUtil.GetLocalIpAddress().ToString(),
                                Port = ConfigManager.Instance.ServerConfig.ServerPort,
                                Congestion = SessionManager.Instance.GetCongestion(),
                            });
                        }
                        await sharedDB.SaveChangesExAsync();
                    }
                }
                catch (Exception e)
                {
                    Log.Error($"[StartServerInfoTask] 서버 정보 업데이트 실패: {e}");
                }
                await Task.Delay(10 * 1000);
            }
        }, cancellationToken);
    }
    private static void ShutdownServer()
    {
        Log.Info("서버 종료 중...");
        _cancellationTokenSource.Cancel();
        Log.Info("서버 종료 완료");
    }
    #endregion
}