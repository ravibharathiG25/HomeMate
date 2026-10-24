using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HomeMate.Shared.Messaging;

public static class MessagingExtensions
{
    /// <summary>
    /// Configures MassTransit with RabbitMQ transport and registers IEventBus.
    /// </summary>
    public static IServiceCollection AddEventBus(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<IBusRegistrationConfigurator>? configureConsumers = null)
    {
        var rabbitMqSettings = configuration.GetSection(RabbitMqSettings.SectionName).Get<RabbitMqSettings>()
                               ?? new RabbitMqSettings();

        services.Configure<RabbitMqSettings>(configuration.GetSection(RabbitMqSettings.SectionName));

        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();

            // Allow caller to register consumers, sagas, etc.
            configureConsumers?.Invoke(x);

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rabbitMqSettings.Host, rabbitMqSettings.Port, rabbitMqSettings.VirtualHost, h =>
                {
                    h.Username(rabbitMqSettings.Username);
                    h.Password(rabbitMqSettings.Password);
                });

                // Configure automatic retry policy with exponential backoff
                cfg.UseMessageRetry(r =>
                {
                    r.Exponential(
                        rabbitMqSettings.RetryCount,
                        TimeSpan.FromSeconds(1),
                        TimeSpan.FromSeconds(rabbitMqSettings.RetryIntervalSeconds * 4),
                        TimeSpan.FromSeconds(rabbitMqSettings.RetryIntervalSeconds));
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        services.AddScoped<IEventBus, EventBus>();

        return services;
    }
}
