using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Notifications;

namespace Ihjezly.Application.Notifications.DeleteNotification;

internal sealed class DeleteNotificationHandler : ICommandHandler<DeleteNotificationCommand>
{
    private readonly INotificationRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteNotificationHandler(INotificationRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteNotificationCommand request, CancellationToken cancellationToken)
    {
        var notification = await _repository.GetByIdAsync(request.NotificationId, cancellationToken);
        if (notification is null)
            return Result.Failure(NotificationErrors.NotFound);

        notification.Delete();
        _repository.Delete(notification);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}