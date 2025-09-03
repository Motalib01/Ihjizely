using Ihjezly.Domain.Properties;

namespace Ihjezly.Domain.Booking.Repositories;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Booking>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<Booking>> GetByBusinessOwnerIdAsync(Guid businessOwnerId, CancellationToken cancellationToken = default);
    Task<List<Booking>> GetByClientIdAsync(Guid clientId, CancellationToken cancellationToken = default);


    void Remove(Booking booking);
    void Add(Booking booking);
    void Update(Booking booking);
}