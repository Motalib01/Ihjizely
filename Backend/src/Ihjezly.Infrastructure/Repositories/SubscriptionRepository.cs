using Ihjezly.Domain.Subscriptions;
using Microsoft.EntityFrameworkCore;

namespace Ihjezly.Infrastructure.Repositories;

internal sealed class SubscriptionRepository : ISubscriptionRepository
{
    private readonly ApplicationDbContext _dbContext;

    public SubscriptionRepository(ApplicationDbContext dbContext) => _dbContext = dbContext;

    public void Add(Subscription subscription) => _dbContext.Subscriptions.Add(subscription);

    public void Update(Subscription subscription) => _dbContext.Subscriptions.Update(subscription);

    public async Task<Subscription?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _dbContext.Subscriptions.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<List<Subscription>> GetByBusinessOwnerIdAsync(Guid businessOwnerId, CancellationToken cancellationToken = default) =>
        await _dbContext.Subscriptions.Where(x => x.BusinessOwnerId == businessOwnerId).ToListAsync(cancellationToken);

    public async Task<List<Subscription>> GetAllAsync(CancellationToken cancellationToken = default)=>
         await _dbContext.Subscriptions.ToListAsync(cancellationToken);

    public async Task<Subscription?> GetActiveByBusinessOwnerIdAsync(Guid businessOwnerId, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        return await _dbContext.Subscriptions
            .Where(s => s.BusinessOwnerId == businessOwnerId
                        && s.StartDate <= now
                        && s.EndDate >= now)
            .OrderByDescending(s => s.EndDate)
            .FirstOrDefaultAsync(cancellationToken);
    }



}