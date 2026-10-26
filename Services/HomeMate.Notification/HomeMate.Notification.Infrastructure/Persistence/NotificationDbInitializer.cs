using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HomeMate.Notification.Infrastructure.Persistence;

public static class NotificationDbInitializer
{
    public static async Task InitializeDatabaseAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<NotificationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<NotificationDbContext>>();

        try
        {
            logger.LogInformation("Ensuring HomeMateNotificationDB is created and up-to-date...");
            await context.Database.EnsureCreatedAsync();
            logger.LogInformation("HomeMateNotificationDB initialized successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing HomeMateNotificationDB.");
            throw;
        }
    }
}
