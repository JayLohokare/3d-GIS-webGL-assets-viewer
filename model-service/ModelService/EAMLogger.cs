using Microsoft.Extensions.Logging;
using System;

namespace ModelService
{
    internal class EAMLogger : ILogger
    {
        private string name;

        public EAMLogger(string name)
        {
            this.name = name;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            //throw new NotImplementedException();
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            //throw new NotImplementedException();
        }
    }
}