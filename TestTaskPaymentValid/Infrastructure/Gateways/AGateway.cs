using TestTaskPaymentValid.Domain.Entities;
using TestTaskPaymentValid.Domain.Enums;
using TestTaskPaymentValid.Infrastructure.Interfaces;

namespace TestTaskPaymentValid.Infrastructure.Gateways
{
    public class AGateway : IPaymentGateway
    {
        public string Name => "A GATEWAY";

        public IReadOnlyCollection<Currency> AvailableCurrency =>
            new List<Currency> {
                Currency.USD,
                Currency.EUR,
                Currency.GBP,
                Currency.JPY,
                Currency.CAD,
                Currency.CHF,
                Currency.AUD,
                Currency.RUB
            };

        public Task<decimal> GetCommissionAsync(PaymentRequest request)
        {
            return Task.FromResult(request.Amount * 0.05m);
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
