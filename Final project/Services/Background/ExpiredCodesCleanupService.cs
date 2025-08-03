using Final_project.Models;
using Final_project.Services.TwoFactorService;
using Microsoft.EntityFrameworkCore;

namespace Final_project.Services.Background
{
    public class ExpiredCodesCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ExpiredCodesCleanupService> _logger;

        public ExpiredCodesCleanupService(IServiceProvider serviceProvider, ILogger<ExpiredCodesCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var twoFactorService = scope.ServiceProvider.GetRequiredService<ITwoFactorService>();
                        var context = scope.ServiceProvider.GetRequiredService<AmazonDBContext>();

                        // Clean expired 2FA codes
                        await twoFactorService.CleanExpiredCodesAsync();

                        // Clean old untrusted devices (older than 90 days)
                        var cutoffDate = DateTime.UtcNow.AddDays(-90);
                        var oldDevices = await context.UserDevices
                            .Where(d => d.LastSeen < cutoffDate && !d.IsTrusted).ToListAsync();
                           

                        if (oldDevices.Any())
                        {
                            context.UserDevices.RemoveRange(oldDevices);
                            await context.SaveChangesAsync();
                            _logger.LogInformation($"Cleaned up {oldDevices.Count} old devices");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during cleanup");
                }

                // Run every hour
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}
