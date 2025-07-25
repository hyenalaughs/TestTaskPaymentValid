using TestTaskPaymentValid.Domain.Enums;

namespace TestTaskPaymentValid.Models
{
    public class GatewayNotificationDto
    {
        public Guid PaymentId { get; set; }

        public string GatewayName { get; set; } = null!;

        public PaymentStatus NewStatus { get; set; }

        public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;

        public Dictionary<string, string>? Metadata { get; set; }
    }
}
