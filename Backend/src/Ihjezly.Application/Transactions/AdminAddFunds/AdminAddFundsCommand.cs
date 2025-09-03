using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Transactions.DTO;

namespace Ihjezly.Application.Transactions.AdminAddFunds;

public sealed record AdminAddFundsCommand(
    Guid WalletId,
    decimal Amount,
    string Currency,
    string Description
) : ICommand<TransactionDto>;