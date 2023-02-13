namespace Log
{
    public static class Logger
    {
        public static LogLevel Level { get; private set; }

        private static ILogger mLogger;

        public static void SetLevel(LogLevel level)
        {
            Level = level;
        }

        public static void SetLogger(ILogger logger)
        {
            mLogger = logger;
        }

        private static void Log(LogLevel logLevel, object message)
        {
            if (logLevel < Level)
            {
                return;
            }

            mLogger?.Log(logLevel, message);
        }

        public static void Debug(object message)
        {
            Log(LogLevel.Debug, message);
        }

        public static void Info(object message)
        {
            Log(LogLevel.Info, message);
        }

        public static void Warn(object message)
        {
            Log(LogLevel.Warn, message);
        }

        public static void Error(object message)
        {
            Log(LogLevel.Error, message);
        }

        public static void Assert(bool condition, string message)
        {
            if (!condition)
            {
                mLogger?.Alert(message);
            }
        }
    }
}