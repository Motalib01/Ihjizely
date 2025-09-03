using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Shared;

namespace Ihjezly.Domain.Transactions;

public sealed class Transaction : Entity
{
    public Guid WalletId { get; private set; }
    public Money Amount { get; private set; }
    public DateTime Timestamp { get; private set; }
    public string Description { get; private set; } = string.Empty;

    private Transaction() : base(Guid.NewGuid()) { }

    private Transaction(Guid walletId, Money amount, string description) : this()
    {
        WalletId = walletId;
        Amount = amount;
        Timestamp = DateTime.UtcNow;
        Description = description;
        RaiseDomainEvent(new TransactionCreatedDomainEvent(Id, WalletId, Amount, Timestamp, Description));
    }

    public static Transaction Create(Guid walletId, Money amount, string description)
        => new Transaction(walletId, amount, description);
}