namespace ServerCore.WAL;

public class TransactionManager
{
    private readonly WalLogger _walLogger;
    private long _transactionCounter = 0;

    public TransactionManager(WalLogger walLogger)
    {
        _walLogger = walLogger;
    }

    public long BeginTransaction()
    {
        return Interlocked.Increment(ref _transactionCounter);  // Thread-Safe
    }

    public void LogTransaction(long transactionId, string operation, string data)
    {
        var entry = new WalEntry
        {
            TransactionId = transactionId,
            Operation = operation,
            Data = data
        };
        _walLogger.AppendLog(entry);    // Thread-Safe
    }
}
