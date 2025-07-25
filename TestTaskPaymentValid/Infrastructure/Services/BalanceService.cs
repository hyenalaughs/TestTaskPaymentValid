using TestTaskPaymentValid.Infrastructure.Interfaces;
using TestTaskPaymentValid.Domain.Enums;

namespace TestTaskPaymentValid.Infrastructure.Services
{
    // This is dake service
    public class BalanceService : IBalanceService
    {
        public async Task<bool> HasSufficientBalanceAsync(string accountId, 
                                                          Currency currency, 
                                                          decimal amount)
        {
            return await Task.FromResult(true);
        }
    }
}
