using HomeMate.Booking.Domain.Enums;

namespace HomeMate.Booking.Application.DTOs;

public record BookingDto(
    Guid Id,
    string BookingReference,
    Guid CustomerId,
    string CustomerName,
    string CustomerEmail,
    string CustomerPhone,
    Guid ServiceId,
    string ServiceName,
    DateTime ScheduledDate,
    string TimeSlot,
    string Address,
    string City,
    string PostalCode,
    string? Notes,
    decimal Price,
    decimal Tax,
    decimal TotalPrice,
    BookingStatus Status,
    DateTime CreatedAtUtc
);
