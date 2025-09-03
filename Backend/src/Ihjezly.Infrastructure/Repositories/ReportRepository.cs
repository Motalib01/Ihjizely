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
    
    public void Add(Report report) => _dbContext.Reports.Add(report);

    public void Update(Report report) => _dbContext.Reports.Update(report);

    public void Delete(Report report) => _dbContext.Reports.Remove(report);

    public async Task<Report?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _dbContext.Reports.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

}