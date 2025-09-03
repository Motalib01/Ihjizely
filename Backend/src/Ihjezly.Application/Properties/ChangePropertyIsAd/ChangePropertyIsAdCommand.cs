using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Notifications;
using Ihjezly.Domain.Properties;
using Ihjezly.Domain.Properties.Repositories;
using Ihjezly.Domain.Subscriptions;

namespace Ihjezly.Application.Properties.ChangePropertyIsAd;

public sealed record ChangePropertyIsAdCommand(Guid PropertyId, bool IsAd) : ICommand;

internal sealed class ChangePropertyIsAdHandler : ICommandHandler<ChangePropertyIsAdCommand>
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ChangePropertyIsAdHandler(
        IPropertyRepository propertyRepository,
        INotificationRepository notificationRepository,
        ISubscriptionRepository subscriptionRepository,
        IUnitOfWork unitOfWork)
    {
        _propertyRepository = propertyRepository;
        _notificationRepository = notificationRepository;
        _subscriptionRepository = subscriptionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ChangePropertyIsAdCommand request, CancellationToken cancellationToken)
    {
        var property = await _propertyRepository.GetByIdNonGeniricAsync(request.PropertyId, cancellationToken);
        if (property is null)
            return Result.Failure(PropertyErrors.NotFound);

        // only check subscription when enabling ads
        if (!property.IsAd && request.IsAd)
        {
            var subscription = await _subscriptionRepository.GetActiveByBusinessOwnerIdAsync(property.BusinessOwnerId, cancellationToken);
            if (subscription is null)
                return Result.Failure(SubscriptionPlanErrors.NotFound);

            if (!subscription.HasAdQuota)
                return Result.Failure(SubscriptionPlanErrors.MaxAdd);

            // consume quota
            subscription.ConsumeAdQuota();
        }

        property.SetIsAd(request.IsAd);

        // notify business owner
        var notificationMessage = request.IsAd
            ? $"تم تفعيل الإعلان لعقارك '{property.Title}'."
            : $"تم إلغاء الإعلان لعقارك '{property.Title}'.";

        var notification = Notification.Create(property.BusinessOwnerId, notificationMessage);
        _notificationRepository.Add(notification);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
