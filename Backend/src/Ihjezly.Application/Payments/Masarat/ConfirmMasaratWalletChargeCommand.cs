using Ihjezly.Application.Abstractions.Messaging;
using Microsoft.Extensions.Logging;

namespace Ihjezly.Application.Payments.Masarat;

public sealed record ConfirmMasaratWalletChargeCommand(
    Guid WalletId,
    string TransactionId,
    string Otp,
    decimal Amount,
    string Currency,
    string Description
) : ICommand;