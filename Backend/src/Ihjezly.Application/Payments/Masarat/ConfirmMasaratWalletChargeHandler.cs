using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Abstractions.Payment;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Shared;
using Ihjezly.Domain.Transactions;

namespace Ihjezly.Application.Payments.Masarat;

internal sealed class ConfirmMasaratWalletChargeHandler
    : ICommandHandler<ConfirmMasaratWalletChargeCommand>
{
    private readonly IWalletRepository _walletRepo;
    private readonly ITransactionRepository _txRepo;
    private readonly IUnitOfWork _uow;
    private readonly IPaymentServiceFactory _paymentFactory;

    public ConfirmMasaratWalletChargeHandler(
        IWalletRepository walletRepo,
        ITransactionRepository txRepo,
        IPaymentServiceFactory paymentFactory,
        IUnitOfWork uow)
    {
        _walletRepo = walletRepo;
        _txRepo = txRepo;
        _paymentFactory = paymentFactory;
        _uow = uow;
    }

    public async Task<Result> Handle(ConfirmMasaratWalletChargeCommand cmd, CancellationToken ct)
    {
        var wallet = await _walletRepo.GetByIdAsync(cmd.WalletId);
        if (wallet is null) return Result.Failure(WalletErrors.NotFound);

        var masarat = _paymentFactory.GetService(PaymentMethod.Masarat);
        MasaratOtpProvider.SetOtp(cmd.Otp); // injects the OTP used by the Masarat service
        var (confirmed, message) = await masarat.ConfirmPaymentAsync(cmd.TransactionId);

        if (!confirmed)
            return Result.Failure(TransactionErrors.NotFound);

        var money = new Money(cmd.Amount, Currency.FromCode(cmd.Currency));
        wallet.AddFunds(money);

        var transaction = Transaction.Create(wallet.Id, money, cmd.Description);
        _txRepo.Add(transaction);
        await _uow.SaveChangesAsync();

        return Result.Success();
    }
}