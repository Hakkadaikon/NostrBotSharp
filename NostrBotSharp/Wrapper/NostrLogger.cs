using Microsoft.Extensions.Logging;

namespace NostrBotSharp.Wrapper
{
    /// <summary>
    /// Nostr logger
    /// </summary>
    /// <typeparam name="T">Logging target</typeparam>
    public class NostrLogger<T> : ILogger<T>
    {
        public static readonly NostrLogger<T> Instance = new NostrLogger<T>();

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            throw new NotImplementedException();
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            throw new NotImplementedException();
        }
    }
}
