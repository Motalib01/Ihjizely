using Microsoft.EntityFrameworkCore;

namespace Ihjezly.Infrastructure.Repositories;

internal sealed class SubscriptionPlanRepository : ISubscriptionPlanRepository
{
    private readonly ApplicationDbContext _dbContext;

    public SubscriptionPlanRepository(ApplicationDbContext dbContext) => 
        _dbContext = dbContext;

    public void Add(SubscriptionPlan plan) => 
        _dbContext.SubscriptionPlans.Add(plan);

    public void Update(SubscriptionPlan plan) => 
        _dbContext.SubscriptionPlans.Update(plan);

    public async Task<SubscriptionPlan?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _dbContext.SubscriptionPlans.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<List<SubscriptionPlan>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _dbContext.SubscriptionPlans.ToListAsync(cancellationToken);

    public void Remove(SubscriptionPlan plan)=>
        _dbContext.SubscriptionPlans.Remove(plan);
    
}