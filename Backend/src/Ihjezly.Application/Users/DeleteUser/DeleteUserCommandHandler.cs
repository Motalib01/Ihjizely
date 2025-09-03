using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Properties.Repositories;
using Ihjezly.Domain.Users;
using Ihjezly.Domain.Users.Repositories;

namespace Ihjezly.Application.Users.DeleteUser;

internal sealed class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUserCommandHandler(
        IUserRepository userRepository,
        IPropertyRepository propertyRepository,
        IWalletRepository walletRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _propertyRepository = propertyRepository;
        _walletRepository = walletRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            return Result.Failure(UserErrors.NotFound);

        // 1. Delete all properties of this user
        var properties = await _propertyRepository.GetByBusinessOwnerIdAsync(user.Id, cancellationToken);
        foreach (var property in properties)
        {
            _propertyRepository.Remove(property);
        }

        // 2. Delete wallet
        var wallet = await _walletRepository.GetByUserIdAsync(user.Id, cancellationToken);
        if (wallet is not null)
        {
            _walletRepository.Remove(wallet); // you'd add this method
        }

        // 3. Delete the user itself
        user.Delete();
        _userRepository.Remove(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
