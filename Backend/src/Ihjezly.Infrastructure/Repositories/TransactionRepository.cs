using Ihjezly.Domain.Transactions;
using Microsoft.EntityFrameworkCore;

namespace Ihjezly.Infrastructure.Repositories;

internal sealed class TransactionRepository : ITransactionRepository
{
    private readonly ApplicationDbContext _dbContext;

    public TransactionRepository(ApplicationDbContext dbContext) => _dbContext = dbContext;

    public void Add(Transaction transaction) => _dbContext.Transactions.Add(transaction);

    public async Task<Transaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _dbContext.Transactions.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<List<Transaction>> GetByWalletIdAsync(Guid walletId, CancellationToken cancellationToken = default) =>
        await _dbContext.Transactions.Where(x => x.WalletId == walletId).ToListAsync(cancellationToken);
}