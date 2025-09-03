using Ihjezly.Domain.Reviews;

public interface IReviewRepository
{
    Task<Review?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Review>> GetByPropertyIdAsync(Guid propertyId, CancellationToken cancellationToken = default);
    void Add(Review review);
    void Update(Review review);
    void Delete(Review review);
}