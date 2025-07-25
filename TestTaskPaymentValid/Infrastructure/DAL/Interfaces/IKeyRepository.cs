using TestTaskPaymentValid.Models;

namespace TestTaskPaymentValid.Infrastructure.DAL.Interfaces
{
    public interface IKeyRepository
    {
        Task<bool> IsExistAsync(Guid key);
        Task AddAsync(IdempotencyKey key);
    }
}
