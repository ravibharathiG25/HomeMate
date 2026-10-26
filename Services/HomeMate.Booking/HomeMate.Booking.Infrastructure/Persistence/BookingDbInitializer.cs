using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HomeMate.Booking.Infrastructure.Persistence;

public static class BookingDbInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<BookingDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<BookingDbContext>>();

        try
        {
            logger.LogInformation("Ensuring HomeMateBookingDB is created...");
            await context.Database.EnsureCreatedAsync();
            logger.LogInformation("HomeMateBookingDB initialized successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to initialize Booking database.");
            throw;
        }
    }
}
