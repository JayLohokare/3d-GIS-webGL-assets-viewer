﻿using Microsoft.Extensions.Logging;

namespace ModelConversionService
{
    internal class EAMLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string name)
        {
            return new EAMLogger(name);
        }

        public void Dispose()
        {
        }
    }
}