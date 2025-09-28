using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Users.Events;
using MediatR;

public sealed class UserCreatedDomainEventHandler : INotificationHandler<UserCreatedDomainEvent>
{
    private readonly IWalletRepository _walletRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UserCreatedDomainEventHandler(
        IWalletRepository walletRepository,
        IUnitOfWork unitOfWork)
    {
        _walletRepository = walletRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UserCreatedDomainEvent notification, CancellationToken cancellationToken)
    {

        Console.WriteLine($">>> Creating wallet for user {notification.UserId}");


        var wallet = Wallet.Create(notification.UserId);
        _walletRepository.Add(wallet);
        await _unitOfWork.SaveChangesAsync(cancellationToken); 
    }
}