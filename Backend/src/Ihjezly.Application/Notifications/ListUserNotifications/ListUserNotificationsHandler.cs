using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Notifications.DTO;
using Ihjezly.Domain.Abstractions;

namespace Ihjezly.Application.Notifications.ListUserNotifications;

internal sealed class ListUserNotificationsHandler : IQueryHandler<ListUserNotificationsQuery, List<NotificationDto>>
{
    private readonly INotificationRepository _repository;

    public ListUserNotificationsHandler(INotificationRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<List<NotificationDto>>> Handle(ListUserNotificationsQuery request, CancellationToken cancellationToken)
    {
        var notifications = await _repository.GetByUserIdAsync(request.UserId, cancellationToken);

        var result = notifications
            .Select(n => new NotificationDto(n.Id, n.UserId, n.Message, n.SentAt, n.IsRead))
            .ToList();

        return result;
    }
}