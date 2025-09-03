namespace Ihjezly.Domain.Properties.Repositories;

public interface ILocationRepository
{
    Task<List<SelectableLocation>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<SelectableLocation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    void Add(SelectableLocation location);
    void Update(SelectableLocation location);
    void Remove(SelectableLocation location);
}