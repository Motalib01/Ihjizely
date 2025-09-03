using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Shared;

public sealed record WalletFundsDeductedDomainEvent(Guid WalletId, Money AmountDeducted, Money NewBalance) : IDomainEvent;