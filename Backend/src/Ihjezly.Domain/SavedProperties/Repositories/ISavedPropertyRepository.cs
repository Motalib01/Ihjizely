using Ihjezly.Domain.SavedProperties;

public interface ISavedPropertyRepository
{
    Task<SavedProperty?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<SavedProperty>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    void Add(SavedProperty savedProperty);
    void Delete(SavedProperty savedProperty);
}