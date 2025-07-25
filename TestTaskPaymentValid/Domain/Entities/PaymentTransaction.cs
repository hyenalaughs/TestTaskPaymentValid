using TestTaskPaymentValid.Domain.Enums;

namespace TestTaskPaymentValid.Domain.Entities
{
    public class PaymentTransaction
    {
        public Guid Id { get; set; }
        public Guid RequestId { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
    }
}
