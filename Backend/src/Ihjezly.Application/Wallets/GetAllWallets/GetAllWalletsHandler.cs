using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Wallets.DTO;
using Ihjezly.Domain.Abstractions;

namespace Ihjezly.Application.Wallets.GetAllWallets;

internal sealed class GetAllWalletsHandler : IQueryHandler<GetAllWalletsQuery, IReadOnlyList<WalletDto>>
{
    private readonly IWalletRepository _repository;

    public GetAllWalletsHandler(IWalletRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IReadOnlyList<WalletDto>>> Handle(GetAllWalletsQuery request, CancellationToken cancellationToken)
    {
        var wallets = await _repository.GetAllAsync(cancellationToken);

        var dtoList = wallets
            .Select(w => new WalletDto(w.Id, w.UserId, w.Balance.Amount, w.Balance.Currency.Code))
            .ToList();

        return Result.Success<IReadOnlyList<WalletDto>>(dtoList);
    }
}