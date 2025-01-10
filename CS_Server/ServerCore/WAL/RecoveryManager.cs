namespace ServerCore.WAL;

public class RecoveryManager
{
    private readonly WalLogger _walLogger;
    
    public RecoveryManager(WalLogger walLogger)
    {
        _walLogger = walLogger;
    }

    public void Recover()
    {
        List<WalEntry> entries = _walLogger.ReadLogs();
        foreach (var entry in entries)
        {
            Console.WriteLine($"Recovering: {entry.TransactionId}, {entry.Operation}, {entry.Data}");
            
            // TODO: 복구 작업 실행
        }
    }
}
