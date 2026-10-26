using HomeMate.Notification.Application.Common.Interfaces;
using HomeMate.Notification.Infrastructure.Persistence;
using HomeMate.Notification.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HomeMate.Notification.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddNotificationInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<NotificationDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<INotificationLogRepository, NotificationLogRepository>();
        services.AddScoped<IEmailService, EmailService>();

        return services;
    }
}
