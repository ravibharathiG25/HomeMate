using HomeMate.Booking.Domain.Enums;
using HomeMate.Shared.Domain;

namespace HomeMate.Booking.Domain.Entities;

public class Booking : Entity<Guid>
{
    public string BookingReference { get; private set; } = string.Empty;
    public Guid CustomerId { get; private set; }
    public string CustomerName { get; private set; } = string.Empty;
    public string CustomerEmail { get; private set; } = string.Empty;
    public string CustomerPhone { get; private set; } = string.Empty;
    public Guid ServiceId { get; private set; }
    public string ServiceName { get; private set; } = string.Empty;
    public DateTime ScheduledDate { get; private set; }
    public string TimeSlot { get; private set; } = string.Empty;
    public string Address { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;
    public string PostalCode { get; private set; } = string.Empty;
    public string? Notes { get; private set; }
    public decimal Price { get; private set; }
    public decimal Tax { get; private set; }
    public decimal TotalPrice { get; private set; }
    public BookingStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;
    public DateTime? ConfirmedAtUtc { get; private set; }
    public DateTime? CancelledAtUtc { get; private set; }

    private Booking() { }

    public static Booking Create(
        Guid customerId,
        string customerName,
        string customerEmail,
        string customerPhone,
        Guid serviceId,
        string serviceName,
        DateTime scheduledDate,
        string timeSlot,
        string address,
        string city,
        string postalCode,
        string? notes,
        decimal price)
    {
        var randomRef = $"HM-{Random.Shared.Next(1000, 9999)}";
        var tax = Math.Round(price * 0.05m, 2); // 5% service tax
        var total = price + tax;

        return new Booking
        {
            Id = Guid.NewGuid(),
            BookingReference = randomRef,
            CustomerId = customerId,
            CustomerName = customerName,
            CustomerEmail = customerEmail,
            CustomerPhone = customerPhone,
            ServiceId = serviceId,
            ServiceName = serviceName,
            ScheduledDate = scheduledDate.Date,
            TimeSlot = timeSlot,
            Address = address,
            City = city,
            PostalCode = postalCode,
            Notes = notes,
            Price = price,
            Tax = tax,
            TotalPrice = total,
            Status = BookingStatus.Confirmed, // Confirmed on creation
            CreatedAtUtc = DateTime.UtcNow,
            ConfirmedAtUtc = DateTime.UtcNow
        };
    }

    public void Cancel()
    {
        Status = BookingStatus.Cancelled;
        CancelledAtUtc = DateTime.UtcNow;
    }
}
