using TestTaskPaymentValid.Domain.Entities;

namespace TestTaskPaymentValid.Domain.Interfaces
{
    public interface IPaymentValidator
    {
        Task ValidateAsync(PaymentRequest request);
    }
}
