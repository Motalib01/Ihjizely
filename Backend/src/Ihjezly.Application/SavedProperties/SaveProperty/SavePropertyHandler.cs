using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Notifications;
using Ihjezly.Domain.SavedProperties;

namespace Ihjezly.Application.SavedProperties.SaveProperty;

internal sealed class SavePropertyHandler : ICommandHandler<SavePropertyCommand, Guid>
{
    private readonly ISavedPropertyRepository _repository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SavePropertyHandler(
        ISavedPropertyRepository repository,
        INotificationRepository notificationRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(SavePropertyCommand request, CancellationToken cancellationToken)
    {
        var existing = await _repository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (existing.Any(p => p.PropertyId == request.PropertyId))
            return Result.Failure<Guid>(SavedPropertyErrors.AlreadySaved);

        var savedProperty = SavedProperty.Save(request.UserId, request.PropertyId);
        _repository.Add(savedProperty);

        var notificationMessage = "تم حفظ هذا العقار في المفضلة بنجاح.";
        var notification = Notification.Create(request.UserId, notificationMessage);
        _notificationRepository.Add(notification);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return savedProperty.Id;
    }
}