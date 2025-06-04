using BepInEx.Logging;

namespace GorillaFaces.Tools
{
    public class Logging
    {
        public static readonly bool DebugLogExclusive = true;

        public static void Info(object message) => LogMessage(LogLevel.Info, message);

        public static void Warning(object message) => LogMessage(LogLevel.Warning, message);

        public static void Error(object message) => LogMessage(LogLevel.Error, message);

        public static void Fatal(object message) => LogMessage(LogLevel.Fatal, message);

        public static void LogMessage(LogLevel level, object message)
        {
#if DEBUG
            Plugin.MainLogSource?.Log(level, message);
#else
            if (!DebugLogExclusive)
                Plugin.MainLogSource?.Log(level, message);
#endif
        }
    }
}
