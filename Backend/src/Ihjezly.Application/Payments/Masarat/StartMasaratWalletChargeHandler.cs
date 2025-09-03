using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Abstractions.Payment;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Shared;

namespace Ihjezly.Application.Payments.Masarat;

internal sealed class StartMasaratWalletChargeHandler
    : ICommandHandler<StartMasaratWalletChargeCommand, string>
{
    private readonly IWalletRepository _walletRepo;
    private readonly IPaymentServiceFactory _paymentFactory;

    public StartMasaratWalletChargeHandler(IWalletRepository walletRepo, IPaymentServiceFactory paymentFactory)
    {
        _walletRepo = walletRepo;
        _paymentFactory = paymentFactory;
    }

    public async Task<Result<string>> Handle(StartMasaratWalletChargeCommand command, CancellationToken ct)
    {
        var wallet = await _walletRepo.GetByIdAsync(command.WalletId);
        if (wallet is null) return Result.Failure<string>(WalletErrors.NotFound);

        var masarat = _paymentFactory.GetService(PaymentMethod.Masarat);
        var (started, transactionId, error) = await masarat.StartPaymentAsync(
            command.IdentityCard, command.Amount, command.Currency, command.Description);

        return started
            ? Result.Success(transactionId!)
            : Result.Failure<string>(TransactionErrors.NotFound);
    }
}