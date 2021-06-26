using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace Decorators.Tests
{
    public class LoggerMock<T> : ILogger<T>
    {
        public List<string> Logged { get; } = new();

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, [NotNull] Func<TState, Exception, string> formatter)
        {
            if (formatter == null) 
                throw new ArgumentNullException(nameof(formatter));

            Logged.Add(formatter(state, exception));
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        public IDisposable BeginScope<TState>(TState state)
        {
            return new NoDisposing();
        }
    }

    public class NoDisposing : IDisposable
    {
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
