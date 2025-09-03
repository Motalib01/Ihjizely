using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Transactions.DTO;
using Ihjezly.Domain.Abstractions;

namespace Ihjezly.Application.Transactions.GetByUser;

internal sealed class GetTransactionsByUserIdHandler
    : IQueryHandler<GetTransactionsByUserIdQuery, List<TransactionDto>>
{
    private readonly IWalletRepository _walletRepository;
    private readonly ITransactionRepository _transactionRepository;

    public GetTransactionsByUserIdHandler(
        IWalletRepository walletRepository,
        ITransactionRepository transactionRepository)
    {
        _walletRepository = walletRepository;
        _transactionRepository = transactionRepository;
    }

    public async Task<Result<List<TransactionDto>>> Handle(
        GetTransactionsByUserIdQuery request,
        CancellationToken cancellationToken)
    {
        var wallet = await _walletRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (wallet is null)
            return Result.Failure<List<TransactionDto>>(WalletErrors.NotFound);

        var transactions = await _transactionRepository.GetByWalletIdAsync(wallet.Id, cancellationToken);

        var result = transactions.Select(tx =>
            new TransactionDto(
                tx.Id,
                tx.WalletId,
                tx.Amount.Amount,
                tx.Amount.Currency.Code,
                tx.Timestamp,
                tx.Description
            )).ToList();


        return result;
    }
}