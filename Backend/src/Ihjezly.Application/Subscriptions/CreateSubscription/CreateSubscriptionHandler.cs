using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Notifications;
using Ihjezly.Domain.Subscriptions;

namespace Ihjezly.Application.Subscriptions.CreateSubscription;

internal sealed class CreateSubscriptionHandler : ICommandHandler<CreateSubscriptionCommand, Guid>
{
    private readonly ISubscriptionRepository _repository;
    private readonly ISubscriptionPlanRepository _planRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateSubscriptionHandler(
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

    public async Task<Result<Guid>> Handle(CreateSubscriptionCommand request, CancellationToken cancellationToken)
    {
        // 1. Get the plan
        var plan = await _planRepository.GetByIdAsync(request.PlanId, cancellationToken);
        if (plan is null)
            return Result.Failure<Guid>(SubscriptionErrors.PlanNotFound);

        // 2. Get the wallet of the business owner
        var wallet = await _walletRepository.GetByUserIdAsync(request.BusinessOwnerId, cancellationToken);
        if (wallet is null)
            return Result.Failure<Guid>(WalletErrors.NotFound);

        // 3. Deduct the subscription price
        try
        {
            wallet.DeductFunds(plan.Price);
            _walletRepository.Update(wallet);
        }
        catch (InvalidOperationException)
        {
            return Result.Failure<Guid>(WalletErrors.InsufficientBalance);
        }

        // 4. Create the subscription
        var subscription = Subscription.Create(request.BusinessOwnerId, plan, request.StartDate);
        _repository.Add(subscription);

        // 5. Create a notification
        var notificationMessage = $"تم تفعيل اشتراكك في '{plan.Name}' ابتداء من {request.StartDate:MMMM dd, yyyy}.";
        var notification = Notification.Create(request.BusinessOwnerId, notificationMessage);
        _notificationRepository.Add(notification);

        // 6. Persist all changes
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return subscription.Id;
    }
}
