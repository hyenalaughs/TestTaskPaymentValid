using TestTaskPaymentValid.Domain.Entities;

namespace TestTaskPaymentValid.Infrastructure.DAL.Interfaces
{
    public interface IRequestRepository
    {
        Task AddAsync(PaymentRequest request);
    }
}
