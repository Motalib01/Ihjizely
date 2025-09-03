using Ihjezly.Domain.SavedProperties;
using Microsoft.EntityFrameworkCore;

namespace Ihjezly.Infrastructure.Repositories;

internal sealed class SavedPropertyRepository : ISavedPropertyRepository
{
    private readonly ApplicationDbContext _dbContext;

    public SavedPropertyRepository(ApplicationDbContext dbContext) => _dbContext = dbContext;

    public void Add(SavedProperty savedProperty) => _dbContext.SavedProperties.Add(savedProperty);

    public void Delete(SavedProperty savedProperty) => _dbContext.SavedProperties.Remove(savedProperty);

    public async Task<SavedProperty?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _dbContext.SavedProperties.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<List<SavedProperty>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await _dbContext.SavedProperties.Where(x => x.UserId == userId).ToListAsync(cancellationToken);
}