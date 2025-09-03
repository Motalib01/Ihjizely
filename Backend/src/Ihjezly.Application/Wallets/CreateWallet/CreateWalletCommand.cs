using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Wallets.DTO;

namespace Ihjezly.Application.Wallets.CreateWallet;

public sealed record CreateWalletCommand(Guid UserId) : ICommand<WalletDto>;

