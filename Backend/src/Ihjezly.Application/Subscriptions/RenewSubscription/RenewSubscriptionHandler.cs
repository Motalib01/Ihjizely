using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Notifications;
using Ihjezly.Domain.Subscriptions;

namespace Ihjezly.Application.Subscriptions.RenewSubscription;

internal sealed class RenewSubscriptionHandler : ICommandHandler<RenewSubscriptionCommand>
{
    private readonly ISubscriptionRepository _repository;
    private readonly ISubscriptionPlanRepository _planRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RenewSubscriptionHandler(
        ISubscriptionRepository repository,
        ISubscriptionPlanRepository planRepository,
        IWalletRepository walletRepository,
        INotificationRepository notificationRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _planRepository = planRepository;
        _walletRepository = walletRepository;
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RenewSubscriptionCommand request, CancellationToken cancellationToken)
    {
        // 1. Get the subscription
        var subscription = await _repository.GetByIdAsync(request.SubscriptionId, cancellationToken);
        if (subscription is null)
            return Result.Failure(SubscriptionErrors.SubscriptionNotFound);

        // 2. Get the plan
        var plan = await _planRepository.GetByIdAsync(request.PlanId, cancellationToken);
        if (plan is null)
            return Result.Failure(SubscriptionErrors.PlanNotFound);

        // 3. Get the wallet of the business owner
        var wallet = await _walletRepository.GetByUserIdAsync(subscription.BusinessOwnerId, cancellationToken);
        if (wallet is null)
            return Result.Failure(WalletErrors.NotFound);

        // 4. Deduct the plan price
        try
        {
            wallet.DeductFunds(plan.Price);
            _walletRepository.Update(wallet);
        }
        catch (InvalidOperationException)
        {
            return Result.Failure(WalletErrors.InsufficientBalance);
        }

        // 5. Renew the subscription
        subscription.Renew(plan);
        _repository.Update(subscription);

        // 6. Create a notification
        var notificationMessage = $"تم تجديد اشتراكك في خطة '{plan.Name}'.";
        var notification = Notification.Create(subscription.BusinessOwnerId, notificationMessage);
        _notificationRepository.Add(notification);

        // 7. Persist all changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
