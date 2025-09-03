using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Properties;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Notifications;
using Ihjezly.Domain.Properties;
using Ihjezly.Domain.Properties.Repositories;
using Ihjezly.Domain.Subscriptions;
using Ihjezly.Domain.Users.Repositories;
using MediatR;

internal sealed class CreateResidencePropertyHandler<TResidence, TDetails>
    : ICommandHandler<CreateResidencePropertyCommand<TResidence, TDetails>, Guid>
    where TResidence : ResidenceProperty<TDetails>, new()
{
    private readonly CreatePropertyHandler<TResidence, TDetails> _handler;
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISubscriptionRepository _subscriptionRepository;

    public CreateResidencePropertyHandler(
        CreatePropertyHandler<TResidence, TDetails> handler,
        INotificationRepository notificationRepository,
        IUnitOfWork unitOfWork,
        ISubscriptionRepository subscriptionRepository)
    {
        _handler = handler;
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task<Result<Guid>> Handle(CreateResidencePropertyCommand<TResidence, TDetails> request, CancellationToken cancellationToken)
    {
        Subscription? subscription = null;

        // Only check subscription if this is an ad
        if (request.IsAd)
        {
            subscription = await _subscriptionRepository.GetActiveByBusinessOwnerIdAsync(request.BusinessOwnerId, cancellationToken);
            if (subscription is null)
                return Result.Failure<Guid>(SubscriptionPlanErrors.NotFound);

            if (!subscription.HasAdQuota)
                return Result.Failure<Guid>(SubscriptionPlanErrors.MaxAdd);
        }

        // Create the property
        var command = new CreatePropertyCommand<TResidence, TDetails>(
            Title: request.Title,
            Description: request.Description,
            Location: request.Location,
            Price: request.Price,
            Currency: request.Currency,
            Details: request.Details,
            ViedeoUrl: request.ViedeoUrl,
            Type: request.Type,
            BusinessOwnerId: request.BusinessOwnerId,
            IsAd: request.IsAd,
            Images: request.Images,
            Discount: request.Discount,
            Unavailables: request.Unavailables ?? new List<DateTime>(),
            Facilities: request.Facilities ?? new List<Facility>()
        );

        var result = await _handler.Handle(command, cancellationToken);

        if (result.IsSuccess)
        {
            // Consume quota only if it was an ad
            subscription?.ConsumeAdQuota();

            // Notification for business owner
            var notification = Notification.Create(
                request.BusinessOwnerId,
                request.IsAd
                    ? $"تم إنشاء إعلان عقار السكن '{request.Title}' بنجاح."
                    : $"تم إنشاء عقار السكن '{request.Title}' بنجاح."
            );

            _notificationRepository.Add(notification);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return result;
    }
}
