using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Notifications;
using Ihjezly.Domain.Properties;
using Ihjezly.Domain.Properties.Repositories;

namespace Ihjezly.Application.Properties.ChangePropertyStatusNonGeneric;

internal sealed class ChangePropertyStatusNonGenericHandler : ICommandHandler<ChangePropertyStatusNonGenericCommand>
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ChangePropertyStatusNonGenericHandler(
        IPropertyRepository propertyRepository,
        INotificationRepository notificationRepository,
        IUnitOfWork unitOfWork)
    {
        _propertyRepository = propertyRepository;
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ChangePropertyStatusNonGenericCommand request, CancellationToken cancellationToken)
    {
        var property = await _propertyRepository.GetByIdNonGeniricAsync(request.PropertyId, cancellationToken);
        if (property is null)
            return Result.Failure(PropertyErrors.NotFound);

        property.ChangeStatus(request.NewStatus);

        var userIdToNotify = property.BusinessOwnerId; 

        var notificationMessage = $"تم تغيير حالة عقارك '{property.Title}' إلى {request.NewStatus}.";

        var notification = Notification.Create(userIdToNotify, notificationMessage);

        _notificationRepository.Add(notification);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}