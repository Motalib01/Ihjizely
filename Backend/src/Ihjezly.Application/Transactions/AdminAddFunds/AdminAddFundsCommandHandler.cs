using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Transactions.DTO;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Shared;
using Ihjezly.Domain.Transactions;

namespace Ihjezly.Application.Transactions.AdminAddFunds;

public class AdminAddFundsCommandHandler : ICommandHandler<AdminAddFundsCommand, TransactionDto>
{
    private readonly IWalletRepository _walletRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AdminAddFundsCommandHandler(
        IWalletRepository walletRepository,
        ITransactionRepository transactionRepository,
        IUnitOfWork unitOfWork)
    {
        _walletRepository = walletRepository;
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TransactionDto>> Handle(AdminAddFundsCommand command, CancellationToken cancellationToken)
    {
        var wallet = await _walletRepository.GetByIdAsync(command.WalletId);
        if (wallet is null)
            return Result.Failure<TransactionDto>(TransactionErrors.WalletNotFound);

        wallet.AddFunds(new Money(command.Amount, Currency.FromCode(command.Currency)));

        var transaction = Transaction.Create(wallet.Id, new Money(command.Amount, Currency.FromCode(command.Currency)), command.Description);
        _transactionRepository.Add(transaction);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new TransactionDto(
            transaction.Id,
            transaction.WalletId,
            transaction.Amount.Amount,
            transaction.Amount.CurrencyCode,
            transaction.Timestamp,
            transaction.Description
        ));
    }

}