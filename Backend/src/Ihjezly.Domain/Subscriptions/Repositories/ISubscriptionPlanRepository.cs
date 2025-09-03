using Ihjezly.Domain.Subscriptions;

public interface ISubscriptionPlanRepository
{
    Task<SubscriptionPlan?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<SubscriptionPlan>> GetAllAsync(CancellationToken cancellationToken = default);
    void Add(SubscriptionPlan plan);
    void Update(SubscriptionPlan plan);
    void Remove(SubscriptionPlan plan);
}