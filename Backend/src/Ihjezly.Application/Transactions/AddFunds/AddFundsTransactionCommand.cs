using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Transactions.DTO;
using Ihjezly.Domain.Shared;

namespace Ihjezly.Application.Transactions.AddFunds;

public sealed record AddFundsTransactionCommand(
    Guid WalletId,
    decimal Amount,
    string Currency,
    string Description,
    PaymentMethod PaymentMethod
) : ICommand<TransactionDto>;