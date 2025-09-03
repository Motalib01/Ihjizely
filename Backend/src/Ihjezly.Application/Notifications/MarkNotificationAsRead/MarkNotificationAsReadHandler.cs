using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Notifications;

namespace Ihjezly.Application.Notifications.MarkNotificationAsRead;

internal sealed class MarkNotificationAsReadHandler : ICommandHandler<MarkNotificationAsReadCommand>
{
    private readonly INotificationRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public MarkNotificationAsReadHandler(INotificationRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
    {
        var notification = await _repository.GetByIdAsync(request.NotificationId, cancellationToken);
        if (notification is null)
            return Result.Failure(NotificationErrors.NotFound);

        notification.MarkAsRead();
        _repository.Update(notification);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
