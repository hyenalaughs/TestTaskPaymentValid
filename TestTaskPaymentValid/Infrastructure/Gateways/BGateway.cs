using TestTaskPaymentValid.Domain.Entities;
using TestTaskPaymentValid.Domain.Enums;
using TestTaskPaymentValid.Infrastructure.Interfaces;

namespace TestTaskPaymentValid.Infrastructure.Gateways
{
    // This is fake gateway
    public class BGateway : IPaymentGateway
    {
        public string Name => "B GATEWAY";

        public IReadOnlyCollection<Currency> AvailableCurrency => 
            new List<Currency> { Currency.EUR, Currency.USD, Currency.RUB};

        public Task<decimal> GetCommissionAsync(PaymentRequest request)
        {
            return Task.FromResult(request.Amount * 0.1m);
        }

        public Task<bool> IsAvailableAsync()
        {
            return Task.FromResult(true);
        }

        public Task<PaymentResult> ProcessAsync(PaymentRequest request)
        {

            return Task.FromResult(new PaymentResult
            {
                PaymentId = request.Id,
                Status = PaymentStatus.Processed,
                CreatedAt = DateTime.UtcNow,
                Message = "In process."
            });
        }
    }
}
