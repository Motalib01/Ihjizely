namespace Ihjezly.Application.Transactions.DTO;

public sealed record TransactionDto(
    Guid Id,
    Guid WalletId,
    decimal Amount,
    string Currency,
    DateTime Timestamp,
    string Description
);