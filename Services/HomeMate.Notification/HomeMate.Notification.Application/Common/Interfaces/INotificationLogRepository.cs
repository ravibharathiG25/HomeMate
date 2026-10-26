using HomeMate.Notification.Domain.Entities;

namespace HomeMate.Notification.Application.Common.Interfaces;

public interface INotificationLogRepository
{
    Task AddAsync(NotificationLog log, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<NotificationLog>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<NotificationLog>> GetRecentAsync(int count = 50, CancellationToken cancellationToken = default);
}
