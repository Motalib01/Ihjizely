
using Ihjezly.Domain.Transactions;

public interface ITransactionRepository
{
    Task<Transaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<List<Transaction>> GetByWalletIdAsync(Guid walletId, CancellationToken cancellationToken = default);
    void Add(Transaction transaction);
}