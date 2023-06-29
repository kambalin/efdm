using Microsoft.Extensions.Logging;
using System;

namespace EFDM.Test.TestConsole.Utilities
{
    public class ConsoleLogger : ILogger
    {
        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (IsEnabled(logLevel))
            {
                var message = formatter(state, exception);
                Console.WriteLine(message);
            }
            Console.WriteLine(exception?.ToString());
        }
    }
}
