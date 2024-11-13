using System.Runtime.CompilerServices;

namespace ServerCore;

public abstract class LoggerBase
{
    [System.Diagnostics.Conditional("DEBUG")]
    public abstract void Debug(string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0);
    public abstract void Info(string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0);
    public abstract void Warn(string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0);
    public abstract void Error(string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0);
    public abstract void FatalError(string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0);
}
