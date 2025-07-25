namespace TestTaskPaymentValid.Domain.Interfaces
{
    public interface IRetryService
    {
        Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> action, int maxAttempts = 3);
    }
}
