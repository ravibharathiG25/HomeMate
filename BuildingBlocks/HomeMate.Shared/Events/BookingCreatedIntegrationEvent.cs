using HomeMate.Shared.Messaging;

namespace HomeMate.Shared.Events;

/// <summary>
/// Published by HomeMate.Booking when a customer successfully books a service.
/// </summary>
public record BookingCreatedIntegrationEvent : IntegrationEvent
{
    public Guid BookingId { get; init; }
    public string BookingReference { get; init; } = string.Empty;
    public Guid CustomerId { get; init; }
    public string CustomerEmail { get; init; } = string.Empty;
    public string CustomerName { get; init; } = string.Empty;
    public Guid ServiceId { get; init; }
    public string ServiceName { get; init; } = string.Empty;
    public DateTime ScheduledDate { get; init; }
    public string TimeSlot { get; init; } = string.Empty;
    public string ServiceAddress { get; init; } = string.Empty;
    public decimal TotalPrice { get; init; }

    public BookingCreatedIntegrationEvent() { }

    public BookingCreatedIntegrationEvent(
        Guid bookingId,
        string bookingReference,
        Guid customerId,
        string customerEmail,
        string customerName,
        Guid serviceId,
        string serviceName,
        DateTime scheduledDate,
        string timeSlot,
        string serviceAddress,
        decimal totalPrice)
    {
        BookingId = bookingId;
        BookingReference = bookingReference;
        CustomerId = customerId;
        CustomerEmail = customerEmail;
        CustomerName = customerName;
        ServiceId = serviceId;
        ServiceName = serviceName;
        ScheduledDate = scheduledDate;
        TimeSlot = timeSlot;
        ServiceAddress = serviceAddress;
        TotalPrice = totalPrice;
    }
}
