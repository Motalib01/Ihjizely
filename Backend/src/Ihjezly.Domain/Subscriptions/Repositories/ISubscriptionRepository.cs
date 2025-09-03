using Ihjezly.Domain.Subscriptions;

public interface ISubscriptionRepository
{
    Task<Subscription?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Subscription>> GetByBusinessOwnerIdAsync(Guid businessOwnerId, CancellationToken cancellationToken = default);
    Task<List<Subscription>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Subscription?> GetActiveByBusinessOwnerIdAsync(Guid businessOwnerId, CancellationToken cancellationToken = default);


    void Add(Subscription subscription);
    void Update(Subscription subscription);
}