using HomeMate.Notification.Domain.Enums;
using HomeMate.Shared.Domain;

namespace HomeMate.Notification.Domain.Entities;

public class NotificationLog : Entity<Guid>
{
    public Guid UserId { get; private set; }
    public string Recipient { get; private set; } = string.Empty;
    public string Subject { get; private set; } = string.Empty;
    public string Body { get; private set; } = string.Empty;
    public NotificationType Type { get; private set; }
    public NotificationStatus Status { get; private set; }
    public string? ErrorMessage { get; private set; }
    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;
    public DateTime? SentAtUtc { get; private set; }

    private NotificationLog() { }

    public static NotificationLog CreateEmail(
        Guid userId,
        string recipient,
        string subject,
        string body)
    {
        return new NotificationLog
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Recipient = recipient,
            Subject = subject,
            Body = body,
            Type = NotificationType.Email,
            Status = NotificationStatus.Pending,
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    public void MarkAsSent()
    {
        Status = NotificationStatus.Sent;
        SentAtUtc = DateTime.UtcNow;
        ErrorMessage = null;
    }

    public void MarkAsFailed(string errorMessage)
    {
        Status = NotificationStatus.Failed;
        ErrorMessage = errorMessage;
    }
}
