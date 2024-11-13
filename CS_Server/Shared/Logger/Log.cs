using NLog;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ServerCore
{
    public static class Log
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        [Conditional("DEBUG")]
        public static void Debug(string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            string messageWithPosition = $"{message} ({Path.GetFileName(file)}:{line})";
            LogEventInfo logEvent = new LogEventInfo(LogLevel.Debug, logger.Name, messageWithPosition);
            logger.Log(typeof(Log), logEvent);
        }

        public static void Info(string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            string messageWithPosition = $"{message} ({Path.GetFileName(file)}:{line})";
            LogEventInfo logEvent = new LogEventInfo(LogLevel.Info, logger.Name, messageWithPosition);
            logger.Log(typeof(Log), logEvent);
        }

        public static void Warn(string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            string messageWithPosition = $"{message} ({Path.GetFileName(file)}:{line})";
            LogEventInfo logEvent = new LogEventInfo(LogLevel.Warn, logger.Name, messageWithPosition);
            logger.Log(typeof(Log), logEvent);
        }

        public static void Error(string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            string messageWithPosition = $"{message} ({Path.GetFileName(file)}:{line})";
            LogEventInfo logEvent = new LogEventInfo(LogLevel.Error, logger.Name, messageWithPosition);
            logger.Log(typeof(Log), logEvent);
        }

        public static void FatalError(string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            string messageWithPosition = $"{message} ({Path.GetFileName(file)}:{line})";
            LogEventInfo logEvent = new LogEventInfo(LogLevel.Fatal, logger.Name, messageWithPosition);
            logger.Log(typeof(Log), logEvent);
        }

    }
}
