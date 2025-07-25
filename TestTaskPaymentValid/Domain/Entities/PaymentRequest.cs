using System.ComponentModel.DataAnnotations.Schema;
using TestTaskPaymentValid.Domain.Enums;
using System.Text.Json;

namespace TestTaskPaymentValid.Domain.Entities
{
    public class PaymentRequest
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }
        public string? SourceAccount { get; set; }
        public string? DestinationAccount { get; set; }
        public string? MetadataJson { get; set; }

        [NotMapped]
        public Dictionary<string, string>? Metadata 
        {
            get 
            {
                return string.IsNullOrEmpty(MetadataJson)
                    ? new Dictionary<string, string>()
                    : JsonSerializer.Deserialize<Dictionary<string, string>>(MetadataJson);
            }
            set
            {
                MetadataJson = JsonSerializer.Serialize(value);
            }
        }
    }
}
