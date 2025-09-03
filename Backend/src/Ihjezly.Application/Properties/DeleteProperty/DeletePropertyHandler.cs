using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Notifications;
using Ihjezly.Domain.Properties;
using Ihjezly.Domain.Properties.Repositories;

namespace Ihjezly.Application.Properties.DeleteProperty;

internal sealed class DeletePropertyHandler<TProperty, TDetails>
    : ICommandHandler<DeletePropertyCommand<TProperty, TDetails>>
    where TProperty : PropertyWithDetails<TDetails>, new()
{
    private readonly IPropertyRepository<TProperty> _repository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeletePropertyHandler(
        IPropertyRepository<TProperty> repository,
        INotificationRepository notificationRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeletePropertyCommand<TProperty, TDetails> request, CancellationToken cancellationToken)
    {
        var property = await _repository.GetByIdAsync(request.PropertyId, cancellationToken);
        if (property is null)
            return Result.Failure(PropertyErrors.NotFound);

        var userIdToNotify = property.BusinessOwnerId; 
        var notificationMessage = $"تم حذف عقارك '{property.Title}'.";

        var notification = Notification.Create(userIdToNotify, notificationMessage);
        _notificationRepository.Add(notification);

        await _repository.DeleteAsync(property);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}