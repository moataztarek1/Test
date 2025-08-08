namespace KidZone.API.DTOs
{
    public class CreateSubscriptionPlanDto
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int DurationInMonths { get; set; }
    }
}
