using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Shared;

public sealed record WalletFundsAddedDomainEvent(Guid WalletId, Money AmountAdded, Money NewBalance) : IDomainEvent;