using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Notifications;
using Ihjezly.Domain.Properties;
using Ihjezly.Domain.Properties.Repositories;

namespace Ihjezly.Application.Properties.ChangePropertyStatus;

internal sealed class ChangePropertyStatusHandler : ICommandHandler<ChangePropertyStatusCommand, Result>
{
    private readonly IPropertyRepository<Property> _propertyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationRepository _notificationRepository;

    public ChangePropertyStatusHandler(
        IPropertyRepository<Property> propertyRepository,
        IUnitOfWork unitOfWork,
        INotificationRepository notificationRepository)
    {
        _propertyRepository = propertyRepository;
        _unitOfWork = unitOfWork;
        _notificationRepository = notificationRepository;
    }

    public async Task<Result<Result>> Handle(ChangePropertyStatusCommand request, CancellationToken cancellationToken)
    {
        var property = await _propertyRepository.GetByIdAsync(request.PropertyId, cancellationToken);
        if (property is null)
            return Result.Failure(PropertyErrors.NotFound);

        property.ChangeStatus(request.NewStatus);

        //Create notification for the business owner
        var message = $"تم {request.NewStatus.ToArabic()} اضافة وحدتك بنجاح";
        var notification = Notification.Create(property.BusinessOwnerId, message);
        _notificationRepository.Add(notification);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

}