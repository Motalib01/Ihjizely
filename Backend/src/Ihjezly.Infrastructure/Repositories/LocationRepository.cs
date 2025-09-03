using Ihjezly.Domain.Properties;
using Ihjezly.Domain.Properties.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ihjezly.Infrastructure.Repositories;

public class LocationRepository : ILocationRepository
{
    private readonly ApplicationDbContext _context;

    public LocationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<SelectableLocation>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.SelectableLocations.ToListAsync(cancellationToken);

    public async Task<SelectableLocation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.SelectableLocations.FindAsync(new object[] { id }, cancellationToken);

    public void Add(SelectableLocation location) => _context.SelectableLocations.Add(location);

    public void Update(SelectableLocation location) => _context.SelectableLocations.Update(location);

    public void Remove(SelectableLocation location) => _context.SelectableLocations.Remove(location);
}