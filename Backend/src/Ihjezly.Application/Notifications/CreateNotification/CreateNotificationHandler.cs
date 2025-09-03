using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Notifications;

namespace Ihjezly.Application.Notifications.CreateNotification;

internal sealed class CreateNotificationHandler : ICommandHandler<CreateNotificationCommand, Guid>
{
    private readonly INotificationRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateNotificationHandler(INotificationRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
    {
        var notification = Notification.Create(request.UserId, request.Message);

        _repository.Add(notification);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return notification.Id;
    }
}