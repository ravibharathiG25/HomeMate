using HomeMate.Booking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HomeMate.Booking.Infrastructure.Persistence;

public class BookingDbContext : DbContext
{
    public BookingDbContext(DbContextOptions<BookingDbContext> options)
        : base(options)
    {
    }

    public DbSet<Domain.Entities.Booking> Bookings => Set<Domain.Entities.Booking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Domain.Entities.Booking>(builder =>
        {
            builder.ToTable("Bookings");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.BookingReference)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(b => b.CustomerName)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(b => b.CustomerEmail)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(b => b.CustomerPhone)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(b => b.ServiceName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(b => b.TimeSlot)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(b => b.Address)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(b => b.City)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(b => b.PostalCode)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(b => b.Notes)
                .HasMaxLength(1000);

            builder.Property(b => b.Price)
                .HasColumnType("decimal(18,2)");

            builder.Property(b => b.Tax)
                .HasColumnType("decimal(18,2)");

            builder.Property(b => b.TotalPrice)
                .HasColumnType("decimal(18,2)");

            builder.Property(b => b.Status)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.HasIndex(b => b.BookingReference)
                .IsUnique();

            builder.HasIndex(b => b.CustomerId);
            builder.HasIndex(b => b.ScheduledDate);
        });
    }
}
