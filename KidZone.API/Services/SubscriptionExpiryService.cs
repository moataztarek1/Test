using KidZone.API.Data;
using KidZone.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace KidZone.API.Services
{
    public class SubscriptionExpiryService: BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<SubscriptionExpiryService> _logger;

        public SubscriptionExpiryService(IServiceScopeFactory scopeFactory, ILogger<SubscriptionExpiryService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Subscription Expiry Service Started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                await CheckExpiredSubscriptions();
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken); //Exe every 24 hour
            }
        }

        private async Task CheckExpiredSubscriptions()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DataContext>();

                var now = DateTime.UtcNow;
                var expiredSubscriptions = await context.UserSubscriptions
                    .Where(s => s.Status == SubscriptionStatus.Active && s.End_Date < now)
                    .ToListAsync();

                if (expiredSubscriptions.Any())
                {
                    foreach (var sub in expiredSubscriptions)
                    {
                        sub.Status = SubscriptionStatus.Expired;
                    }

                    await context.SaveChangesAsync();
                    _logger.LogInformation($"{expiredSubscriptions.Count} subscriptions marked as Expired.");
                }
            }
        }
    }
}
