using KidZone.Domain.Enums;

namespace KidZone.API.DTOs
{
    public class SubscriptionRequestDto
    {
        public int PlanId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
    }
}
