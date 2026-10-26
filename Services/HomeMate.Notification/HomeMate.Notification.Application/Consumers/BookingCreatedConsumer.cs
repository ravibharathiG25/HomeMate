using HomeMate.Notification.Application.Common.Interfaces;
using HomeMate.Notification.Domain.Entities;
using HomeMate.Shared.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace HomeMate.Notification.Application.Consumers;

public class BookingCreatedConsumer : IConsumer<BookingCreatedIntegrationEvent>
{
    private readonly IEmailService _emailService;
    private readonly INotificationLogRepository _repository;
    private readonly ILogger<BookingCreatedConsumer> _logger;

    public BookingCreatedConsumer(
        IEmailService emailService,
        INotificationLogRepository repository,
        ILogger<BookingCreatedConsumer> logger)
    {
        _emailService = emailService;
        _repository = repository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<BookingCreatedIntegrationEvent> context)
    {
        var message = context.Message;

        _logger.LogInformation(
            "Consuming BookingCreatedIntegrationEvent for Reference: {Ref}, Customer: {Email}, Service: {Service}",
            message.BookingReference,
            message.CustomerEmail,
            message.ServiceName);

        var subject = $"Booking Confirmed: {message.ServiceName} (Ref: {message.BookingReference})";

        var body =
            $"Hello {message.CustomerName},\n\n" +
            $"Your booking for {message.ServiceName} has been confirmed!\n\n" +
            $"Booking Details:\n" +
            $"• Reference: {message.BookingReference}\n" +
            $"• Date: {message.ScheduledDate:yyyy-MM-dd}\n" +
            $"• Time Slot: {message.TimeSlot}\n" +
            $"• Address: {message.ServiceAddress}\n" +
            $"• Total: ${message.TotalPrice:F2}\n\n" +
            $"A certified HomeMate professional has been assigned and will arrive during your chosen window.\n\n" +
            $"Best regards,\nThe HomeMate Team";

        var notificationLog = NotificationLog.CreateEmail(
            message.CustomerId,
            message.CustomerEmail,
            subject,
            body);

        try
        {
            await _emailService.SendEmailAsync(message.CustomerEmail, subject, body, context.CancellationToken);
            notificationLog.MarkAsSent();
            _logger.LogInformation("Successfully sent booking confirmation email for {Ref}", message.BookingReference);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send booking confirmation email for {Ref}", message.BookingReference);
            notificationLog.MarkAsFailed(ex.Message);
            throw;
        }
        finally
        {
            await _repository.AddAsync(notificationLog, context.CancellationToken);
        }
    }
}
