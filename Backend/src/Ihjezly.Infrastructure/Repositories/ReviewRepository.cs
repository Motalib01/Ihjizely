using Ihjezly.Domain.Booking;
using Ihjezly.Domain.Reviews;
using Microsoft.EntityFrameworkCore;

namespace Ihjezly.Infrastructure.Repositories;

internal sealed class ReviewRepository : IReviewRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ReviewRepository(ApplicationDbContext dbContext) => _dbContext = dbContext;

    public async Task<Review?> GetByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken) =>
        await _dbContext.Reviews.FirstOrDefaultAsync(x => x.BookingId == bookingId, cancellationToken);

    public void Add(Review review) => _dbContext.Reviews.Add(review);

    public void Update(Review review) => _dbContext.Reviews.Update(review);

    public void Delete(Review review) => _dbContext.Reviews.Remove(review);

    public async Task<Review?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _dbContext.Reviews.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<List<Review>> GetByPropertyIdAsync(Guid propertyId, CancellationToken cancellationToken = default) =>
        await _dbContext.Reviews.Where(x => x.PropertyId == propertyId).ToListAsync(cancellationToken);

    
}