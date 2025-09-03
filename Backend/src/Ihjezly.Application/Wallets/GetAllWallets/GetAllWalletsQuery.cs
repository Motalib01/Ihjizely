using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Wallets.DTO;

namespace Ihjezly.Application.Wallets.GetAllWallets;

public sealed record GetAllWalletsQuery() : IQuery<IReadOnlyList<WalletDto>>;