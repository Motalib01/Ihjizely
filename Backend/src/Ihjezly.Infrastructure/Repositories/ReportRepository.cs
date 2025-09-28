using Ihjezly.Domain.NewFolder;
using Ihjezly.Domain.Reposrts.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ihjezly.Infrastructure.Repositories;

internal sealed class ReportRepository : IReportRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ReportRepository(ApplicationDbContext dbContext) => _dbContext = dbContext;
    public async Task<IReadOnlyList<Report>> GetAllAsync(CancellationToken cancellationToken = default)=>
         await _dbContext.Reports.ToListAsync(cancellationToken);

    public async Task<Report?> GetByIdWithUserAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Reports
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }


    public void Add(Report report) => _dbContext.Reports.Add(report);

    public void Update(Report report) => _dbContext.Reports.Update(report);

    public void Delete(Report report) => _dbContext.Reports.Remove(report);

    public async Task<Report?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _dbContext.Reports.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

}