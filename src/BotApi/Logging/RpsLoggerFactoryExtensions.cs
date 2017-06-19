using System;
using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Logging.Abstractions;

namespace BotApi.Logging
{
    public static class RpsLoggerFactoryExtensions
    {
        public static ILoggerFactory AddRps(this ILoggerFactory factory)
        {
            return AddRps(factory, LogLevel.Information);
        }

        public static ILoggerFactory AddRps(this ILoggerFactory factory, Func<string, LogLevel, EventId, bool> filter)
        {
            factory.AddProvider(new RpsProvider(filter));
            return factory;
        }

        public static ILoggerFactory AddRps(this ILoggerFactory factory, LogLevel minLevel)
        {
            return AddRps(factory, (category, logLevel, eventId) => logLevel >= minLevel);
        }
    }
}
