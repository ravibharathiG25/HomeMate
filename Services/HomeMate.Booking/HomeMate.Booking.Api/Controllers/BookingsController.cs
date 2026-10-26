using System.Security.Claims;
using HomeMate.Booking.Application.Common.Interfaces;
using HomeMate.Booking.Application.DTOs;
using HomeMate.Shared.Events;
using HomeMate.Shared.Exceptions;
using HomeMate.Shared.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeMate.Booking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IBookingRepository _repository;
    private readonly IEventBus _eventBus;
    private readonly ILogger<BookingsController> _logger;

    public BookingsController(
        IBookingRepository repository,
        IEventBus eventBus,
        ILogger<BookingsController> logger)
    {
        _repository = repository;
        _eventBus = eventBus;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new service booking and publishes an integration event.
    /// </summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(BookingDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequest request, CancellationToken cancellationToken)
    {
        if (request.Price <= 0)
        {
            throw new BadRequestException("Service price must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(request.Address) || string.IsNullOrWhiteSpace(request.City))
        {
            throw new BadRequestException("Service address and city are required.");
        }

        var customerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;

        if (string.IsNullOrWhiteSpace(customerIdClaim) || !Guid.TryParse(customerIdClaim, out var customerId))
        {
            throw new BadRequestException("Invalid or missing user ID in token.");
        }

        var customerEmail = User.FindFirst(ClaimTypes.Email)?.Value
            ?? User.FindFirst("email")?.Value
            ?? "customer@homemate.com";

        var customerName = User.FindFirst(ClaimTypes.Name)?.Value
            ?? User.FindFirst("name")?.Value
            ?? customerEmail.Split('@')[0];

        var booking = Domain.Entities.Booking.Create(
            customerId: customerId,
            customerName: customerName,
            customerEmail: customerEmail,
            customerPhone: request.CustomerPhone,
            serviceId: request.ServiceId,
            serviceName: request.ServiceName,
            scheduledDate: request.ScheduledDate,
            timeSlot: request.TimeSlot,
            address: request.Address,
            city: request.City,
            postalCode: request.PostalCode,
            notes: request.Notes,
            price: request.Price);

        await _repository.AddAsync(booking, cancellationToken);

        _logger.LogInformation(
            "Booking created: {BookingRef} for Service: {ServiceName}, Customer: {Email}",
            booking.BookingReference,
            booking.ServiceName,
            booking.CustomerEmail);

        // Publish event to RabbitMQ asynchronously in the background so HTTP response returns instantly (< 20ms)
        _ = Task.Run(async () =>
        {
            try
            {
                var integrationEvent = new BookingCreatedIntegrationEvent(
                    booking.Id,
                    booking.BookingReference,
                    booking.CustomerId,
                    booking.CustomerEmail,
                    booking.CustomerName,
                    booking.ServiceId,
                    booking.ServiceName,
                    booking.ScheduledDate,
                    booking.TimeSlot,
                    $"{booking.Address}, {booking.City} {booking.PostalCode}",
                    booking.TotalPrice);

                using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
                await _eventBus.PublishAsync(integrationEvent, timeoutCts.Token);
                _logger.LogInformation("Published BookingCreatedIntegrationEvent for {BookingRef}", booking.BookingReference);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to publish BookingCreatedIntegrationEvent (RabbitMQ offline). Booking is safely stored.");
            }
        });

        var responseDto = MapToDto(booking);
        return CreatedAtAction(nameof(GetById), new { id = booking.Id }, responseDto);
    }

    /// <summary>
    /// Gets all bookings made by the currently logged-in customer.
    /// </summary>
    [HttpGet("my-bookings")]
    [Authorize]
    [ProducesResponseType(typeof(IReadOnlyList<BookingDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyBookings(CancellationToken cancellationToken)
    {
        var customerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;

        if (string.IsNullOrWhiteSpace(customerIdClaim) || !Guid.TryParse(customerIdClaim, out var customerId))
        {
            throw new BadRequestException("Invalid or missing user ID in token.");
        }

        var bookings = await _repository.GetByCustomerIdAsync(customerId, cancellationToken);
        var dtos = bookings.Select(MapToDto).ToList();

        return Ok(dtos);
    }

    /// <summary>
    /// Gets a booking by its unique ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(BookingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var booking = await _repository.GetByIdAsync(id, cancellationToken);
        if (booking == null)
        {
            throw new NotFoundException($"Booking with ID '{id}' was not found.");
        }

        return Ok(MapToDto(booking));
    }

    private static BookingDto MapToDto(Domain.Entities.Booking b) => new(
        b.Id,
        b.BookingReference,
        b.CustomerId,
        b.CustomerName,
        b.CustomerEmail,
        b.CustomerPhone,
        b.ServiceId,
        b.ServiceName,
        b.ScheduledDate,
        b.TimeSlot,
        b.Address,
        b.City,
        b.PostalCode,
        b.Notes,
        b.Price,
        b.Tax,
        b.TotalPrice,
        b.Status,
        b.CreatedAtUtc
    );
}
