namespace TestTaskPaymentValid.Models
{
    public class IdempotencyKey
    {
        public Guid Id { get; set; }
        public Guid TransactionId { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
