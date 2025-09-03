using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Wallets.DTO;
using Ihjezly.Domain.Abstractions;

namespace Ihjezly.Application.Wallets.GetWalletByUser;

internal sealed class GetWalletByUserHandler : IQueryHandler<GetWalletByUserQuery, WalletDto>
{
    private readonly IWalletRepository _repository;

    public GetWalletByUserHandler(IWalletRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<WalletDto>> Handle(GetWalletByUserQuery request, CancellationToken cancellationToken)
    {
        var wallet = await _repository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (wallet is null) return Result.Failure<WalletDto>(WalletErrors.NotFound);

        return new WalletDto(
            wallet.Id,
            wallet.UserId,
            wallet.Balance.Amount,
            wallet.Balance.Currency.Code
        );
    }
}