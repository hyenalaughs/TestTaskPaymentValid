using TestTaskPaymentValid.Domain.Interfaces;

namespace TestTaskPaymentValid.Application.Services
{
    public class RetryService : IRetryService
    {
        private readonly ILogger<RetryService> _logger;

        public RetryService(ILogger<RetryService> logger)
        {
            _logger = logger;
        }

        public async Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> action, int maxAttempts = 3)
        {
            int attempt = 0;
            Exception? lastException = null;

            while (attempt < maxAttempts)
            {
                try
                {
                    return await action();
                }
                catch (Exception ex)
                {
                    lastException = ex;
                    attempt++;

                    _logger.LogWarning(ex, "Attempt {Attempt} failed. Retrying...", attempt);

                    var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt)); // 2, 4, 8 сек
                    await Task.Delay(delay);
                }
            }

            _logger.LogError(lastException, "All retry attempts failed.");
            throw new Exception("Retry limit exceeded", lastException);
        }
    }
}
