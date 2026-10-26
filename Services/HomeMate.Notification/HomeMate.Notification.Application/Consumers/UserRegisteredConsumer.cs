using HomeMate.Notification.Application.Common.Interfaces;
using HomeMate.Notification.Domain.Entities;
using HomeMate.Shared.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace HomeMate.Notification.Application.Consumers;

public class UserRegisteredConsumer : IConsumer<UserRegisteredIntegrationEvent>
{
    private readonly IEmailService _emailService;
    private readonly INotificationLogRepository _repository;
    private readonly ILogger<UserRegisteredConsumer> _logger;

    public UserRegisteredConsumer(
        IEmailService emailService,
        INotificationLogRepository repository,
        ILogger<UserRegisteredConsumer> logger)
    {
        _emailService = emailService;
        _repository = repository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UserRegisteredIntegrationEvent> context)
    {
        var message = context.Message;

        _logger.LogInformation(
            "Consuming UserRegisteredIntegrationEvent for UserId: {UserId}, Email: {Email}, Role: {Role}",
            message.UserId,
            message.Email,
            message.Role);

        var subject = message.Role == "Provider"
            ? "Welcome to HomeMate Provider Network!"
            : "Welcome to HomeMate — Your Trusted Home Services Partner!";

        var greetingName = !string.IsNullOrWhiteSpace(message.FirstName)
            ? message.FirstName
            : "there";

        var body = message.Role == "Provider"
            ? $"Hello {greetingName},\n\nThank you for joining HomeMate as a Service Provider! Your profile is active. You can now access your provider dashboard and start receiving verified local service requests.\n\nBest regards,\nThe HomeMate Team"
            : $"Hello {greetingName},\n\nWelcome to HomeMate! Your customer account is ready. Explore top-rated plumbers, electricians, cleaners, and more right at your fingertips.\n\nBest regards,\nThe HomeMate Team";

        var notificationLog = NotificationLog.CreateEmail(
            message.UserId,
            message.Email,
            subject,
            body);

        try
        {
            await _emailService.SendEmailAsync(message.Email, subject, body, context.CancellationToken);
            notificationLog.MarkAsSent();
            _logger.LogInformation("Successfully sent welcome email to {Email}", message.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send welcome email to {Email}", message.Email);
            notificationLog.MarkAsFailed(ex.Message);
            throw; // Let MassTransit trigger its retry policy
        }
        finally
        {
            await _repository.AddAsync(notificationLog, context.CancellationToken);
        }
    }
}
