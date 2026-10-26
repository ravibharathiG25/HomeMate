using BookingEntity = HomeMate.Booking.Domain.Entities.Booking;

namespace HomeMate.Booking.Application.Common.Interfaces;

public interface IBookingRepository
{
    Task AddAsync(BookingEntity booking, CancellationToken cancellationToken = default);
    Task<BookingEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BookingEntity>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BookingEntity>> GetAllAsync(CancellationToken cancellationToken = default);
}
