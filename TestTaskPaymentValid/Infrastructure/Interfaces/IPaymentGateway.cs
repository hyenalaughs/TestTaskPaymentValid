using TestTaskPaymentValid.Domain.Entities;
using TestTaskPaymentValid.Domain.Enums;

namespace TestTaskPaymentValid.Infrastructure.Interfaces
{
    public interface IPaymentGateway
    {
        string Name { get; }
        IReadOnlyCollection<Currency> AvailableCurrency { get; }

        Task<decimal> GetCommissionAsync(PaymentRequest request);
        Task<PaymentResult> ProcessAsync(PaymentRequest request);
        Task<bool> IsAvailableAsync();
    }
}
