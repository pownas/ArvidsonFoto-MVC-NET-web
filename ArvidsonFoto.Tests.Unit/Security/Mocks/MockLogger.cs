using Microsoft.Extensions.Logging;

namespace ArvidsonFoto.Tests.Unit.Security.Mocks;

/// <summary>
/// Mock implementation of ILogger for testing purposes
/// </summary>
public class MockLogger<T> : ILogger<T>
{
    public List<LogEntry> LogEntries { get; } = new();

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        LogEntries.Add(new LogEntry
        {
            LogLevel = logLevel,
            EventId = eventId,
            State = state,
            Exception = exception,
            Message = formatter(state, exception)
        });
    }

    public class LogEntry
    {
        public LogLevel LogLevel { get; init; }
        public EventId EventId { get; init; }
        public object? State { get; init; }
        public Exception? Exception { get; init; }
        public string Message { get; init; } = string.Empty;
    }
}
