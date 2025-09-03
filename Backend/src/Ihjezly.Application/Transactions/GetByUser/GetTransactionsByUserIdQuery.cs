using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Transactions.DTO;

namespace Ihjezly.Application.Transactions.GetByUser;

public sealed record GetTransactionsByUserIdQuery(Guid UserId)
    : IQuery<List<TransactionDto>>;