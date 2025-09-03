using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Transactions.DTO;

namespace Ihjezly.Application.Transactions.ListTransactionsForWallet;

public sealed record ListTransactionsForWalletQuery(Guid WalletId) : IQuery<List<TransactionDto>>;