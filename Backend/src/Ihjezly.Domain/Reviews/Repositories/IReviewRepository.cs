using Ihjezly.Domain.Reviews;

public interface IReviewRepository
{
    Task<Review?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Review>> GetByPropertyIdAsync(Guid propertyId, CancellationToken cancellationToken = default);

    Task<Review?> GetByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken);


    void Add(Review review);
    void Update(Review review);
    void Delete(Review review);
}