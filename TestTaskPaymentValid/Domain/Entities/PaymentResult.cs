using TestTaskPaymentValid.Domain.Enums;

namespace TestTaskPaymentValid.Domain.Entities
{
    public class PaymentResult
    {
        public Guid PaymentId { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? Message { get; set; }
    }
}
