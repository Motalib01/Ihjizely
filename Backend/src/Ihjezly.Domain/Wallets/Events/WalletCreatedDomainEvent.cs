using Ihjezly.Domain.Abstractions;

public sealed record WalletCreatedDomainEvent(Guid WalletId, Guid UserId) : IDomainEvent;