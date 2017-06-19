using System;
using System.Net.Http;
using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Logging.Abstractions;

namespace BotApi.Logging
{
    public class RpsLogger : ILogger
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly Func<string, LogLevel, EventId, bool> _filter;
        private readonly string _name;

        public RpsLogger(string name)
            : this(name, null)
        { }

        public RpsLogger(string name, Func<string, LogLevel, EventId, bool> filter)
        {
            _name = name;
            _filter = filter;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var message = formatter(state, exception);

            var content = new StringContent("{ \"message\": \"{" + message + "}\" }\r\n");

            var response = _httpClient.PostAsync("", content).Result;
        }
    }
}
