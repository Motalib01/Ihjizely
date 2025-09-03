using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Transactions.DTO;

namespace Ihjezly.Application.Transactions.DeductFunds;

public sealed record DeductFundsTransactionCommand(Guid WalletId, decimal Amount, string Currency, string Description)
    : ICommand<TransactionDto>;