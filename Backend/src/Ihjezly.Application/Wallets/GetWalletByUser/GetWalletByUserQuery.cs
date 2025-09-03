using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Wallets.DTO;

namespace Ihjezly.Application.Wallets.GetWalletByUser;

public sealed record GetWalletByUserQuery(Guid UserId) : IQuery<WalletDto>;