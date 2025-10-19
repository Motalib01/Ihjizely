using Ihjezly.Domain.Booking;
using Ihjezly.Domain.Booking.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ihjezly.Infrastructure.Repositories;

internal sealed class BookingRepository : IBookingRepository
{
    private readonly ApplicationDbContext _dbContext;

    public BookingRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(Booking booking) => _dbContext.Bookings.Add(booking);

    public async Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _dbContext.Bookings.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);


    public async Task<List<Booking>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _dbContext.Bookings.ToListAsync(cancellationToken);

    public async Task<List<Booking>> GetByClientIdAsync(Guid clientId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Bookings
            .Where(b => b.ClientId == clientId)
            .ToListAsync(cancellationToken);
    }


    public async Task<List<Booking>> GetByPropertyId(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Bookings
            .Where(b => b.PropertyId == id)
            .ToListAsync(cancellationToken);
    }

    public void Remove(Booking booking)
    {
        _dbContext.Bookings.Remove(booking);

    }

    public void Update(Booking booking)
        => _dbContext.Bookings.Update(booking);
    public async Task<List<Booking>> GetByBusinessOwnerIdAsync(Guid businessOwnerId, CancellationToken cancellationToken = default)
    {
        var query = from booking in _dbContext.Bookings
            join property in _dbContext.Properties on booking.PropertyId equals property.Id
            where property.BusinessOwnerId == businessOwnerId
            select booking;

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<List<Booking>> GetOverlappingBookingsAsync(
        Guid propertyId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Bookings
            .Where(b => b.PropertyId == propertyId
                        && b.Status == BookingStatus.Pending // only pending ones matter
                        && b.StartDate < endDate
                        && b.EndDate > startDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Booking>> GetExpiredConfirmedBookingsAsync(DateTime currentTime, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Bookings
            .Where(b => b.Status == BookingStatus.Confirmed && b.EndDate < currentTime)
            .ToListAsync(cancellationToken);
    }


}