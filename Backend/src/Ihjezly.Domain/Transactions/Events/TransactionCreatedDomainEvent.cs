using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Shared;

public sealed record TransactionCreatedDomainEvent(Guid TransactionId, Guid WalletId, Money Amount, DateTime Timestamp, string Description) : IDomainEvent;