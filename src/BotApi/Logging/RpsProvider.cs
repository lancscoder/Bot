using System;
using Microsoft.Extensions.Logging;

namespace BotApi.Logging
{
    public class RpsProvider : ILoggerProvider
    {
        private readonly Func<string, LogLevel, EventId, bool> _filter;

        public RpsProvider(Func<string, LogLevel, EventId, bool> filter)
        {
            _filter = filter;
        }

        public ILogger CreateLogger(string name)
        {
            return new RpsLogger(name, _filter);
        }

        public void Dispose()
        {
        }
    }
}
