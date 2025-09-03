using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Notifications;
using Ihjezly.Domain.Properties;
using Ihjezly.Domain.Properties.Repositories;

namespace Ihjezly.Application.Properties.DeleteProperty;

internal sealed class DeletePropertyNonGenericHandler : ICommandHandler<DeletePropertyNonGenericCommand>
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeletePropertyNonGenericHandler(
        IPropertyRepository propertyRepository,
        INotificationRepository notificationRepository,
        IUnitOfWork unitOfWork)
    {
        _propertyRepository = propertyRepository;
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeletePropertyNonGenericCommand request, CancellationToken cancellationToken)
    {
        var property = await _propertyRepository.GetByIdNonGeniricAsync(request.PropertyId, cancellationToken);

        if (property is null)
            return Result.Failure(PropertyErrors.NotFound);

        var userIdToNotify = property.BusinessOwnerId; 
        var notificationMessage = $"تم حذف عقارك '{property.Title}'.";

        var notification = Notification.Create(userIdToNotify, notificationMessage);
        _notificationRepository.Add(notification);

        _propertyRepository.Remove(property);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}