using Microsoft.EntityFrameworkCore;

namespace Ihjezly.Infrastructure.Repositories;

internal sealed class WalletRepository : IWalletRepository
{
    private readonly ApplicationDbContext _dbContext;

    public WalletRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(Wallet wallet) => _dbContext.Wallets.Add(wallet);

    public void Update(Wallet wallet) => _dbContext.Wallets.Update(wallet);

    public async Task<Wallet?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _dbContext.Wallets.FirstOrDefaultAsync(w => w.Id == id, cancellationToken);

    public async Task<Wallet?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await _dbContext.Wallets.FirstOrDefaultAsync(w => w.UserId == userId, cancellationToken);

    public async Task<IReadOnlyList<Wallet>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _dbContext.Wallets.ToListAsync(cancellationToken);

    public void Remove(Wallet wallet) => 
        _dbContext.Wallets.Remove(wallet);
}