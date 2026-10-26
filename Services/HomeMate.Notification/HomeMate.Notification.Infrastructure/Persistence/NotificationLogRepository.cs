using HomeMate.Notification.Application.Common.Interfaces;
using HomeMate.Notification.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HomeMate.Notification.Infrastructure.Persistence;

public class NotificationLogRepository : INotificationLogRepository
{
    private readonly NotificationDbContext _context;

    public NotificationLogRepository(NotificationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(NotificationLog log, CancellationToken cancellationToken = default)
    {
        await _context.NotificationLogs.AddAsync(log, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<NotificationLog>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.NotificationLogs
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<NotificationLog>> GetRecentAsync(int count = 50, CancellationToken cancellationToken = default)
    {
        return await _context.NotificationLogs
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .Take(count)
            .ToListAsync(cancellationToken);
    }
}
