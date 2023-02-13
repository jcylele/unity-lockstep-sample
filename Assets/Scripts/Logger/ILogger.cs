namespace Log
{
    public enum LogLevel
    {
        None,
        Debug,
        Info,
        Warn,
        Error,
    }

    public interface ILogger
    {
        void Log(LogLevel logLevel, object message);

        void Alert(string message);
    }
}