using KidZone.Domain.Enums;

namespace KidZone.API.DTOs
{
    public class PaymentDto
    {
        public int SubscriptionId { get; set; }
        public PaymentMethod Method { get; set; }
    }
}
