public interface IWalletRepository
{
    Task<Wallet?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Wallet?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Wallet>> GetAllAsync(CancellationToken cancellationToken = default);

    void Add(Wallet wallet);
    void Update(Wallet wallet);

    void Remove(Wallet wallet);
}