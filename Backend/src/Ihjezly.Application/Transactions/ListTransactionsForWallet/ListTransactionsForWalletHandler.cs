using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Transactions.DTO;
using Ihjezly.Domain.Abstractions;

namespace Ihjezly.Application.Transactions.ListTransactionsForWallet;

internal sealed class ListTransactionsForWalletHandler : IQueryHandler<ListTransactionsForWalletQuery, List<TransactionDto>>
{
    private readonly ITransactionRepository _repository;

    public ListTransactionsForWalletHandler(ITransactionRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<List<TransactionDto>>> Handle(ListTransactionsForWalletQuery request, CancellationToken cancellationToken)
    {
        var transactions = await _repository.GetByWalletIdAsync(request.WalletId, cancellationToken);

        var dtos = transactions.Select(t => new TransactionDto(
            t.Id,
            t.WalletId,
            t.Amount.Amount,
            t.Amount.Currency.Code,
            t.Timestamp,
            t.Description
        )).ToList();

        return dtos;
    }
}