namespace TestTaskPaymentValid.Models
{
    public class PaymentDto
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; } = null!;
        public string SourceAccount { get; set; } = null!;
        public string DestinationAccount { get; set; } = null!;
        public Dictionary<string, string> Metadata { get; set; } = new();
    }
}
