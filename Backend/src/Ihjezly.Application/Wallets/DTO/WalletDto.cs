namespace Ihjezly.Application.Wallets.DTO;

public sealed record WalletDto(Guid WalletId, Guid UserId, decimal Amount, string Currency);