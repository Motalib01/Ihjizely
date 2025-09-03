using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Transactions.DTO;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Notifications;
using Ihjezly.Domain.Shared;
using Ihjezly.Domain.Transactions;

namespace Ihjezly.Application.Transactions.DeductFunds;

internal sealed class DeductFundsTransactionCommandHandler : ICommandHandler<DeductFundsTransactionCommand, TransactionDto>
{
    private readonly IWalletRepository _walletRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeductFundsTransactionCommandHandler(
        IWalletRepository walletRepository,
        ITransactionRepository transactionRepository,
        INotificationRepository notificationRepository,
        IUnitOfWork unitOfWork)
    {
        _walletRepository = walletRepository;
        _transactionRepository = transactionRepository;
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TransactionDto>> Handle(DeductFundsTransactionCommand command, CancellationToken cancellationToken)
    {
        var wallet = await _walletRepository.GetByIdAsync(command.WalletId, cancellationToken);
        if (wallet is null)
            return Result.Failure<TransactionDto>(TransactionErrors.NotFound);

        var money = new Money(command.Amount, Currency.FromCode(command.Currency));

        try
        {
            wallet.DeductFunds(money);
        }
        catch (InvalidOperationException)
        {
            return Result.Failure<TransactionDto>(WalletErrors.InsufficientBalance);
        }

        var transaction = Transaction.Create(command.WalletId, money, command.Description);
        _transactionRepository.Add(transaction);

        var message = $"An amount of {money.Amount} {money.Currency.Code} has been deducted from your wallet.";
        var notification = Notification.Create(wallet.UserId, message);
        _notificationRepository.Add(notification);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = new TransactionDto(
            transaction.Id,
            transaction.WalletId,
            transaction.Amount.Amount,
            transaction.Amount.Currency.Code,
            transaction.Timestamp,
            transaction.Description
        );

        return Result.Success(dto);
    }
}