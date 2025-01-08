using System.Text.Json;

namespace ServerCore.WAL;

public class WalLogger
{
    private readonly string _logFilePath;
    private readonly object _lock = new object();

    public WalLogger(string logFilePath)
    {
        _logFilePath = logFilePath;
    }

    // 로그 기록
    public void AppendLog(WalEntry entry)
    {
        lock(_lock)
        {
            using (StreamWriter writer = new StreamWriter(new FileStream(_logFilePath, FileMode.Append)))
            {
                string json = JsonSerializer.Serialize(entry);
                writer.WriteLine(json);
            }
        }
    }
    // 로그 읽기
    public List<WalEntry> ReadLogs()
    {
        lock (_lock)
        {
            List<WalEntry> entries = new List<WalEntry>();
            using (StreamReader reader = new StreamReader(new FileStream(_logFilePath, FileMode.Open)))
            {
                while (reader.EndOfStream == false)
                {
                    string json = reader.ReadLine();
                    WalEntry entry = JsonSerializer.Deserialize<WalEntry>(json);
                    entries.Add(entry);
                }
            }
            return entries;
        }
    }
}
