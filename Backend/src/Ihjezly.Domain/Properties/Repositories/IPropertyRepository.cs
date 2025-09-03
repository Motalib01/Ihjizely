using Ihjezly.Application.Properties.PropertySearch;

namespace Ihjezly.Domain.Properties.Repositories;

public interface IPropertyRepository<TProperty> where TProperty : Property
{
    Task<List<TProperty>> GetAllAcceptedAsync(CancellationToken cancellationToken = default);
    Task<List<TProperty>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TProperty?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Add(TProperty property);
    Task<List<TProperty>> GetByStatusAsync(PropertyStatus status, CancellationToken cancellationToken = default);
    Task DeleteAsync(TProperty property);


}

public interface IPropertyRepository
{
    
    Task<Property?> GetByIdNonGeniricAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Property>> GetAllNonGenericAsync(CancellationToken cancellationToken = default);
    Task<List<Property>> GetAllAcceptedNonGenericAsync(CancellationToken cancellationToken = default);
    Task<List<Property>> GetByBusinessOwnerIdAsync(Guid businessOwnerId, CancellationToken cancellationToken = default);
    void Remove(Property property);

    Task<List<Property>> SearchAsync(PropertySearchRequest request, CancellationToken cancellationToken = default);


}