using System.Runtime.CompilerServices;

namespace ServerCore;

public class NLogLogger : LoggerBase
{
    public override void Debug(string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
    {
        Log.Debug(message, file, line);
    }
    public override void Info(string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
    {
        Log.Info(message, file, line);
    }
    public override void Warn(string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
    {
        Log.Warn(message, file, line);
    }
    public override void Error(string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
    {
        Log.Error(message, file, line);
    }

    public override void FatalError(string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
    {
        Log.FatalError(message, file, line);
    }
}
