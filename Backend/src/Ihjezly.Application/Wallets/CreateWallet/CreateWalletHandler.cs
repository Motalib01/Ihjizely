using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Wallets.DTO;
using Ihjezly.Domain.Abstractions;

namespace Ihjezly.Application.Wallets.CreateWallet;

internal sealed class CreateWalletHandler : ICommandHandler<CreateWalletCommand, WalletDto>
{
    private readonly IWalletRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateWalletHandler(IWalletRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<WalletDto>> Handle(CreateWalletCommand request, CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (existing is not null)
            return Result.Failure<WalletDto>(WalletErrors.AlreadyExists);

        var wallet = new Wallet(request.UserId);
        _repository.Add(wallet);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new WalletDto(wallet.Id, wallet.UserId, wallet.Balance.Amount, wallet.Balance.Currency.Code);
    }
}