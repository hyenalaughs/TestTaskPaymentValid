using TestTaskPaymentValid.Domain.Enums;

namespace TestTaskPaymentValid.Infrastructure.Interfaces
{
    public interface IBalanceService
    {
        Task<bool> HasSufficientBalanceAsync(string accountId, 
                                             Currency currency, 
                                             decimal amount);
    }
}
