using KidZone.API.Data;
using KidZone.Domain.Entities;
using KidZone.Domain.Enums;

namespace KidZone.API.Seeders
{
    public static class DataSeeder
    {
        public static void Seed(DataContext context, int userId)
        {
            if (!context.SubscriptionPlans.Any())
            {
                var plans = new List<SubscriptionPlan>
            {
                new SubscriptionPlan { Name = "Basic Plan", Price = 100, DurationInMonths = 1 },
                new SubscriptionPlan { Name = "Standard Plan", Price = 250, DurationInMonths = 3 },
                new SubscriptionPlan { Name = "Premium Plan", Price = 900, DurationInMonths = 12 },
            };

                context.SubscriptionPlans.AddRange(plans);
                context.SaveChanges();
            }

            if (!context.UserSubscriptions.Any())
            {
                var basicPlan = context.SubscriptionPlans.First(p => p.Name == "Basic Plan");

                var userSub = new UserSubscription
                {
                    UserID = userId,
                    Sub_Plan_ID = basicPlan.ID,
                    Status = SubscriptionStatus.Active,
                    Start_Date = DateTime.UtcNow,
                    End_Date = DateTime.UtcNow.AddMonths(basicPlan.DurationInMonths)
                };

                context.UserSubscriptions.Add(userSub);
                context.SaveChanges();

                var payment = new Payment
                {
                    Amount = basicPlan.Price,
                    Method = PaymentMethod.Visa,
                    Status = PaymentStatus.Completed,
                    Date = DateTime.UtcNow,
                    User_Sub_ID = userSub.ID
                };

                context.Payments.Add(payment);
                context.SaveChanges();
            }
        }
    }
}
