namespace HomeMate.Booking.Application.DTOs;

public record CreateBookingRequest(
    Guid ServiceId,
    string ServiceName,
    DateTime ScheduledDate,
    string TimeSlot,
    string Address,
    string City,
    string PostalCode,
    string CustomerPhone,
    string? Notes,
    decimal Price
);
