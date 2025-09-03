using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Shared;

public sealed class Wallet : Entity
{
    public Guid UserId { get; private set; }
    public Money Balance { get; private set; } = Money.Zero();

    private Wallet() : base(Guid.NewGuid()) { }

    public Wallet(Guid userId) : this()
    {
        UserId = userId;
        RaiseDomainEvent(new WalletCreatedDomainEvent(Id, UserId));
    }

    public static Wallet Create(Guid userId)
        => new Wallet(userId);

    public void AddFunds(Money amount)
    {
        Balance += amount;
        RaiseDomainEvent(new WalletFundsAddedDomainEvent(Id, amount, Balance));
    }

    public void DeductFunds(Money amount)
    {
        if (!Balance.IsGreaterThanOrEqual(amount))
            throw new InvalidOperationException("Insufficient balance.");

        Balance -= amount;
        RaiseDomainEvent(new WalletFundsDeductedDomainEvent(Id, amount, Balance));
    }

}