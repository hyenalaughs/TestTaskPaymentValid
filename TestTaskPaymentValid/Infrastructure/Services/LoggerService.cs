using StackExchange.Redis;
using System.Text.Json;

namespace TestTaskPaymentValid.Infrastructure.Services
{
    public class LoggerService<T> : ILogger<T>
    {
        private readonly IDatabase _database;
        private readonly string _serviceName;

        public LoggerService(IDatabase database)
        {
            _database = database;
            _serviceName = typeof(T).FullName ?? "Unknown serviese";
        }

        public IDisposable? BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, 
                                EventId eventId, 
                                TState state, 
                                Exception? exception, 
                                Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;

            var logEntry = new
            {
                Timestamp = DateTime.UtcNow,
                Category = _serviceName,
                Level = logLevel.ToString(),
                EventId = eventId.Id,
                Message = formatter(state, exception),
                Exception = exception?.ToString()
            };

            string serialized = JsonSerializer.Serialize(logEntry);

            _database.ListRightPush("logs", serialized);
        }
    }
}
