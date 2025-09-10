using Ihjezly.Application.Properties.PropertySearch;
using Ihjezly.Domain.Properties;
using Ihjezly.Domain.Properties.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ihjezly.Infrastructure.Repositories;

internal sealed class PropertyRepository<TProperty> : IPropertyRepository<TProperty>, IPropertyRepository
    where TProperty : Property
{
    private readonly ApplicationDbContext _dbContext;

    public PropertyRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    // Non-generic methods for Property
    public async Task<Property?> GetByIdNonGeniricAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Properties
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<List<Property>> GetAllNonGenericAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Properties
            .ToListAsync(cancellationToken);
    }

    public async Task DeleteAsync(TProperty property)
    {
        _dbContext.Set<TProperty>().Remove(property);
    }
    public void Remove(Property property)
    {
        _dbContext.Properties.Remove(property);
    }

    public async Task<List<Property>> GetAllAcceptedNonGenericAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Properties
            .Where(p => p.Status == PropertyStatus.Accepted)
            .ToListAsync(cancellationToken);
    }

    //Generic methods for TProperty
    public async Task<List<TProperty>> GetAllAcceptedAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<TProperty>()
            .Where(p => p.Status == PropertyStatus.Accepted)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<TProperty>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<TProperty>()
            .ToListAsync(cancellationToken);
    }

    public void Add(TProperty property) => _dbContext.Set<TProperty>().Add(property);

    public async Task<TProperty?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _dbContext.Set<TProperty>().FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<List<TProperty>> GetByStatusAsync(PropertyStatus status, CancellationToken cancellationToken)
    {
        return await _dbContext.Properties
            .OfType<TProperty>()
            .Where(p => p.Status == status)
            .ToListAsync(cancellationToken);
    }


    public async Task<List<Property>> GetByBusinessOwnerIdAsync(Guid businessOwnerId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Properties
            .Where(p => p.BusinessOwnerId == businessOwnerId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Property>> SearchAsync(PropertySearchRequest request, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Properties.AsQueryable();

        // Title search
        if (!string.IsNullOrWhiteSpace(request.Title))
            query = query.Where(p => p.Title.Contains(request.Title));

        // Property type filter
        if (request.Types != null && request.Types.Any())
        {
            var loweredTypes = request.Types.Select(t => t.ToLower()).ToList();

            query = query.Where(p =>
                (loweredTypes.Contains("apartment") && p is Apartment) ||
                (loweredTypes.Contains("chalet") && p is Chalet) ||
                (loweredTypes.Contains("hotelroom") && p is HotelRoom) ||
                (loweredTypes.Contains("hotelapartment") && p is HotelApartment) ||
                (loweredTypes.Contains("eventhalllarge") && p is EventHallLarge) ||
                (loweredTypes.Contains("eventhallsmall") && p is EventHallSmall) ||
                (loweredTypes.Contains("meetingroom") && p is MeetingRoom) ||
                (loweredTypes.Contains("resort") && p is Resort) ||
                (loweredTypes.Contains("resthouse") && p is RestHouse) ||
                (loweredTypes.Contains("villaevent") && p is VillaEvent)
            );
        }


        // Price filters
        if (request.MinPrice.HasValue)
            query = query.Where(p => p.Price.Amount >= request.MinPrice.Value);
        if (request.MaxPrice.HasValue)
            query = query.Where(p => p.Price.Amount <= request.MaxPrice.Value);

        // Status filter
        if (request.Status.HasValue)
            query = query.Where(p => p.Status == request.Status.Value);

        // Business owner filter
        if (request.BusinessOwnerId.HasValue)
            query = query.Where(p => p.BusinessOwnerId == request.BusinessOwnerId.Value);

        // Execute query first
        var properties = await query.ToListAsync(cancellationToken);

        // Facilities filter (client-side)
        if (request.Facility.HasValue)
            properties = properties
                .Where(p => p.Facilities.Contains(request.Facility.Value))
                .ToList();

        if (!string.IsNullOrWhiteSpace(request.City))
            properties = properties
                .Where(p => p.Location.City.Equals(request.City, StringComparison.OrdinalIgnoreCase))
                .ToList();

        if (!string.IsNullOrWhiteSpace(request.State))
            properties = properties
                .Where(p => p.Location.State.Equals(request.State, StringComparison.OrdinalIgnoreCase))
                .ToList();

        return properties;
    }

    public void Update(Property property)
    {
        _dbContext.Properties.Update(property);
    }
}