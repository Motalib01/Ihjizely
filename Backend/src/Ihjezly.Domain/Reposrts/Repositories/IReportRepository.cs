using Ihjezly.Domain.NewFolder;

namespace Ihjezly.Domain.Reposrts.Repositories;

public interface IReportRepository
{
    Task<Report?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Report>> GetAllAsync(CancellationToken cancellationToken = default);

    void Add(Report report);
    void Update(Report report);
    void Delete(Report report);
}