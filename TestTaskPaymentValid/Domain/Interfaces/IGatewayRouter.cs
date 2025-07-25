using TestTaskPaymentValid.Infrastructure.Interfaces;
using TestTaskPaymentValid.Domain.Entities;

namespace TestTaskPaymentValid.Domain.Interfaces
{
    public interface IGatewayRouter
    {
        Task<IPaymentGateway> SelectGatewayAsync(PaymentRequest request);
    }
}
