using TestTaskPaymentValid.Domain.Entities;
using TestTaskPaymentValid.Models;

namespace TestTaskPaymentValid.Domain.Interfaces
{
    public interface IPaymentProcessor
    {
        Task<PaymentResult> ProcessAsync(PaymentDto request);
        Task<PaymentResult?> GetStatusAsync(Guid paymentId);
        Task HandleNotificationaAsync(GatewayNotificationDto notification);
    }
}
