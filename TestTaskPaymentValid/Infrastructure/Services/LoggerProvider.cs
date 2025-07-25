using StackExchange.Redis;
using System.Text.Json;

namespace TestTaskPaymentValid.Infrastructure.Services
{
    public class LoggerProvider : ILoggerProvider
    {
        private readonly IDatabase _database;

        public LoggerProvider(IDatabase redisDb)
        {
            _database = redisDb;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new RedisLoggerGeneric(_database, categoryName);
        }

        public void Dispose() { }

        private class RedisLoggerGeneric : ILogger
        {
            private readonly IDatabase _redisDb;
            private readonly string _categoryName;

            public RedisLoggerGeneric(IDatabase redisDb, string categoryName)
            {
                _redisDb = redisDb;
                _categoryName = categoryName;
            }

            public IDisposable? BeginScope<TState>(TState state) => null;

            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(
                LogLevel logLevel,
                EventId eventId,
                TState state,
                Exception? exception,
                Func<TState, Exception?, string> formatter)
            {
                if (!IsEnabled(logLevel)) return;

                var logEntry = new
                {
                    Timestamp = DateTime.UtcNow,
                    Category = _categoryName,
                    Level = logLevel.ToString(),
                    EventId = eventId.Id,
                    Message = formatter(state, exception),
                    Exception = exception?.ToString()
                };

                string serialized = JsonSerializer.Serialize(logEntry);
                _redisDb.ListRightPush("logs", serialized);
            }
        }
    }
}
