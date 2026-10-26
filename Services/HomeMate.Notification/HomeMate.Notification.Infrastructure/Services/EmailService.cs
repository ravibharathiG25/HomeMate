using HomeMate.Notification.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace HomeMate.Notification.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "\n" +
            "======================================================================\n" +
            "                    [MOCK EMAIL DISPATCHER]                           \n" +
            "======================================================================\n" +
            "TO:       {To}\n" +
            "SUBJECT:  {Subject}\n" +
            "DATE:     {Date}\n" +
            "----------------------------------------------------------------------\n" +
            "{Body}\n" +
            "======================================================================\n",
            to,
            subject,
            DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC"),
            body);

        return Task.CompletedTask;
    }
}
