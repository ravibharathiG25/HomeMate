using HomeMate.Notification.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HomeMate.Notification.Infrastructure.Persistence;

public class NotificationDbContext : DbContext
{
    public NotificationDbContext(DbContextOptions<NotificationDbContext> options)
        : base(options)
    {
    }

    public DbSet<NotificationLog> NotificationLogs => Set<NotificationLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<NotificationLog>(builder =>
        {
            builder.ToTable("NotificationLogs");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Recipient)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(x => x.Subject)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.Body)
                .IsRequired();

            builder.Property(x => x.Type)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(x => x.ErrorMessage)
                .HasMaxLength(2000);

            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.CreatedAtUtc);
        });
    }
}
