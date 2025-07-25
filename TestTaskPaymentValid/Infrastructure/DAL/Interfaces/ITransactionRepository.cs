using TestTaskPaymentValid.Domain.Entities;

namespace TestTaskPaymentValid.Infrastructure.DAL.Interfaces
{
    public interface ITransactionRepository
    {
        Task<PaymentTransaction?> GetByIdAsync(Guid id);
        Task AddAsync(PaymentTransaction transaction);
        Task UpdateAsync(PaymentTransaction transaction);
        Task SaveChangesAsync();
    }
}
