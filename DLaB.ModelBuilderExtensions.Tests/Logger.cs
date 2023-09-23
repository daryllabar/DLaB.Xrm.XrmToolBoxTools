using Microsoft.Extensions.Logging;
using System;

namespace DLaB.ModelBuilderExtensions.Tests
{
    public class Logger : ILogger
    {
        public static Logger Instance = new Logger();

        public LogLevel LogLevel { get; set; } = LogLevel.Information;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            Console.WriteLine($"[{eventId.Id,2}: {logLevel,-12}] - {formatter(state, exception)}");
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return LogLevel <= logLevel && logLevel != LogLevel.None;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return default;
        }
    }
}
