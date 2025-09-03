using Ihjezly.Application.Abstractions.Messaging;
using Microsoft.Extensions.Logging;

namespace Ihjezly.Application.Payments.Masarat;

public sealed record StartMasaratWalletChargeCommand(
    Guid WalletId,
    string IdentityCard,
    decimal Amount,
    string Currency,
    string Description
) : ICommand<string>;